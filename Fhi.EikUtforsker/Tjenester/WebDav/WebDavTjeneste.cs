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

        public async Task<WebDavResourceNode> BuildResourceTree(string rootUri, int antallDager)
        {
            var resources = await GetResources(rootUri);
            var limited = resources
                .Where(r => r.LastModifiedDate.HasValue == false 
                    || r.LastModifiedDate > DateTime.Now.AddDays(-antallDager))
                .ToList();
            var folderMap = AddAllFolders(limited);
            var root = BuildResourcesTree(folderMap, limited);
            root = SortResourceTree(root);
            return root;
        }

        private WebDavResourceNode SortResourceTree(WebDavResourceNode root)
        {
            var children = new List<WebDavResourceNode>();
            foreach (var child in root.Children)
            {
                children.Add(SortResourceTree(child));
            }
            var sortedChildren = children.OrderByDescending(c => c.LastModifiedDate).ToList();
            var newNode = new WebDavResourceNode(root.Uri, root.RelUri, root.ETag, root.LastModifiedDate, root.ContentLength, root.IsCollection, sortedChildren);
            return newNode;
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

        public async Task<IList<WebDavFile>> BuildResourceHistory(string rootUri, int antallDager)
        {
            var resources = await GetResources(rootUri);

            var limited = resources
                .Where(r => r.LastModifiedDate.HasValue == false
                    || r.LastModifiedDate > DateTime.Now.AddDays(-antallDager))
                .ToList();

            var history = limited
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

            var resources = await GetResourceAndChildren(client, rootUri);
            var unike = RemoveDuplicates(resources);
            return unike;
        }

        private async Task<List<WebDavResource>> GetResourceAndChildren(WebDavClient client, string uri)
        {
            var resources = new List<WebDavResource>();

            var propfindParameters = new PropfindParameters
            {
                ApplyTo = ApplyTo.Propfind.ResourceAndChildren
            };

            var result = await client.Propfind(uri, propfindParameters);

            if (!result.IsSuccessful)
            {
                _logger.LogError("PropFind({Uri}) was not successfull: StatusCode: {StatusCode}   Description: {Description}", uri, result.StatusCode, result.Description);
                throw new Exception($"PropFind({uri}) was not successfull: StatusCode: {result.StatusCode}   Description: {result.Description}");
            }

            foreach (var resource in result.Resources)
            {
                if (!resource.LastModifiedDate.HasValue || DateTime.Now.AddMonths(-6) > resource.LastModifiedDate.Value) continue;
                resources.Add(resource);
                if (resource.IsCollection && !resource.Uri.Equals(uri, StringComparison.OrdinalIgnoreCase))
                {
                    var resourceAndChildren = await GetResourceAndChildren(client, resource.Uri);
                    resources.AddRange(resourceAndChildren);
                }
            }
            return resources;
        }


        public List<WebDavResource> RemoveDuplicates(List<WebDavResource> resources)
        {
            return resources.GroupBy(obj => obj.Uri)
                .Select(group => group.First())
                .ToList();
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
