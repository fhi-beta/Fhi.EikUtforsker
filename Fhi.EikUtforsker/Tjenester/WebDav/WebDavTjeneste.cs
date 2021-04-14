using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebDav;

namespace Fhi.EikUtforsker.Tjenester.WebDav
{
    public class WebDavTjeneste
    {
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;
        private readonly string _thumbprint;
        private readonly string _baseAddress;
        private readonly IOptions<EikUtforskerOptions> _options;
        private readonly ILogger<WebDavTjeneste> _logger;
        private readonly IMemoryCache _cache;

        public WebDavTjeneste(IOptions<EikUtforskerOptions> options, IMemoryCache memoryCache, ILogger<WebDavTjeneste> logger)
        {
            _storeName = (StoreName)Enum.Parse(typeof(StoreName), options.Value.StoreName);
            _storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), options.Value.StoreLocation);
            _thumbprint = options.Value.Thumbprint;
            _baseAddress = options.Value.BaseAddress;
            _options = options;
            _cache = memoryCache;
            _logger = logger;
        }

        public async Task<WebDavResourceNode> GetResourceTree(string rootUri)
        {
            WebDavResourceNode root;
            var cacheKey = $"ResourceTree_{rootUri}";
            if (!_cache.TryGetValue(cacheKey, out root))
            {
                root = await BuildResourceTree(rootUri);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                _cache.Set(cacheKey, root, cacheEntryOptions);
            }
            return root;
        }

        public async Task<WebDavResourceNode> BuildResourceTree(string rootUri)
        {
            var resources = await GetResources(rootUri);
            var folderMap = AddAllFolders(resources);
            var root = BuildResourcesTree(folderMap, resources);
            return root;
        }

        private static WebDavResourceNode BuildResourcesTree(Dictionary<string, WebDavResourceNode> folderMap, IReadOnlyCollection<WebDavResource> resources)
        {
            WebDavResourceNode root = null;
            foreach (var resource in resources)
            {
                var (parentUri, relUri) = SplitUri(resource.Uri);
                var parentEntry = GetParentEntry(folderMap, parentUri);

                if (parentEntry == null)
                {
                    root = folderMap[resource.Uri];
                    continue;
                }

                var entry = resource.IsCollection ? folderMap[resource.Uri] : new WebDavResourceNode(resource);
                entry.RelUri = relUri;
                var parentFolder = folderMap[parentUri];
                parentFolder.Children.Add(entry);
            }
            return root;
        }

        public async Task<IList<WebDavFile>> GetResourceHistory(string rootUri)
        {
            IList<WebDavFile> history;
            var cacheKey = $"ResourceHistory_{rootUri}";
            if (!_cache.TryGetValue(cacheKey, out history))
            {
                history = await BuildResourceHistory(rootUri);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                _cache.Set(cacheKey, history, cacheEntryOptions);
            }
            return history;
        }

        public async Task<IList<WebDavFile>> BuildResourceHistory(string rootUri)
        {
            var resources = await GetResources(rootUri);

            var history = resources
                .Where(r => r.IsCollection == false)
                .Select(r => new WebDavFile(r, rootUri))
                .OrderByDescending(r=>r.LastModifiedDate)
                .ToList();

            return history;
        }

        private static Dictionary<string, WebDavResourceNode> AddAllFolders(IReadOnlyCollection<WebDavResource> resources)
        {
            var folderMap = new Dictionary<string, WebDavResourceNode>();

            foreach (var resource in resources.Where(r => r.IsCollection))
            {
                var uri = resource.Uri;
                if (folderMap.ContainsKey(uri))
                {
                    throw new Exception($"Duplicated URI: {uri}");
                }
                folderMap.Add(uri, new WebDavResourceNode(resource));
            }
            return folderMap;
        }

        private static WebDavResourceNode GetParentEntry(Dictionary<string, WebDavResourceNode> folderMap, string parentUri)
        {
            if (!folderMap.ContainsKey(parentUri))
            {
                return null;
            }
            return folderMap[parentUri];
        }

        private static (string parentFolderUri, string relUri) SplitUri(string uri)
        {
            var parentFolderUriRegex = new Regex("^(.*/)([^/]+/?)$");
            var match = parentFolderUriRegex.Match(uri);
            if (!match.Success)
            {
                throw new Exception($"Fant ikke parent folder-uri fra uri: {uri}");
            }
            var parentFolderUri = match.Groups[1].Value;
            var relUri = HttpUtility.UrlDecode(match.Groups[2].Value);
            return (parentFolderUri, relUri);
        }

        private async Task<IReadOnlyCollection<WebDavResource>> GetResources(string rootUri)
        {
            var cacheKey = $"Resources_{rootUri}";
            IReadOnlyCollection<WebDavResource> resources;
            if (!_cache.TryGetValue(cacheKey, out resources))
            {
                resources = await GetResourcesFromUri(rootUri);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                _cache.Set(cacheKey, resources, cacheEntryOptions);
            }
            return resources;
        }


        private async Task<IReadOnlyCollection<WebDavResource>> GetResourcesFromUri(string rootUri)
        {
            using var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            };

            try
            {
                var certificate = CertificateHelper.GetCertificate(_storeName, _storeLocation, _thumbprint);
                handler.ClientCertificates.Add(certificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fant ikke sertifikat med thumbprint {_thumbprint} StoreName: {_options.Value.StoreName} StoreLocation: {_options.Value.StoreLocation}");
                return null;
            }

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseAddress),
            };

            using var client = new WebDavClient(httpClient);

            var propfindParameters = new PropfindParameters();
            propfindParameters.ApplyTo = ApplyTo.Propfind.ResourceAndAncestors;

            var result = await client.Propfind(rootUri, propfindParameters);

            return result.Resources;
        }

        public async Task<string> GetFile(string uri, CancellationToken cancellationToken)
        {
            string fil;
            var cacheKey = $"File_{uri}";
            if (!_cache.TryGetValue(cacheKey, out fil))
            {
                fil = await GetFileFromUri(uri, cancellationToken);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                _cache.Set(cacheKey, fil, cacheEntryOptions);
            }
            return fil;
        }

        public async Task<string> GetFileFromUri(string uri, CancellationToken cancellationToken)
        {
            using var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            };

            try
            {
                var certificate = CertificateHelper.GetCertificate(_storeName, _storeLocation, _thumbprint);
                handler.ClientCertificates.Add(certificate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fant ikke sertifikat med thumbprint {_thumbprint} StoreName: {_options.Value.StoreName} StoreLocation: {_options.Value.StoreLocation}");
                return null;
            }

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseAddress),
            };

            using var client = new WebDavClient(httpClient);
            var getFileParameters = new GetFileParameters { CancellationToken = cancellationToken };
            var response = await client.GetRawFile(uri, getFileParameters);
            var reader = new StreamReader(response.Stream);
            var fil = await reader.ReadToEndAsync();
            return fil;
        }
    }
}
