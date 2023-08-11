using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using System.Reflection.PortableExecutable;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml.Linq;
using WebDav;
using WebDav.Client;

namespace Fhi.EikUtforsker.Tjenester.WebDav;

public class WebDavTjeneste
{
    private readonly StoreName _storeName;
    private readonly StoreLocation _storeLocation;
    private readonly string _thumbprint;
    private readonly string _baseAddress;
    private readonly IOptions<EikUtforskerOptions> _options;
    private readonly ILogger<WebDavTjeneste> _logger;

    public WebDavTjeneste(IOptions<EikUtforskerOptions> options, ILogger<WebDavTjeneste> logger)
    {
        _storeName = (StoreName)Enum.Parse(typeof(StoreName), options.Value.StoreName);
        _storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), options.Value.StoreLocation);
        _thumbprint = options.Value.Thumbprint;
        _baseAddress = options.Value.BaseAddress;
        _options = options;
        _logger = logger;
    }

    public async Task<List<WebDavResourceNode>> GetResources(string uri, ApplyTo.Propfind applyTo = ApplyTo.Propfind.ResourceAndChildren)
    {
        var resources = await GetResourcesFromUri(uri, applyTo);
        var result = resources.Select(r => new WebDavResourceNode(r))
            .OrderBy(r=>r.Uri)
            .ToList();
        return result;
    }

    private async Task<IReadOnlyCollection<WebDavResource>> GetResourcesFromUri(string uri, ApplyTo.Propfind applyTo)
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

        var resources = await GetResources(client, uri, applyTo);
        return resources;
    }

    private async Task<List<WebDavResource>> GetResources(WebDavClient client, string uri, ApplyTo.Propfind applyTo)
    {
        var resources = new List<WebDavResource>();

        var propfindParameters = new PropfindParameters
        {
            ApplyTo = applyTo
        };

        var result = await client.Propfind(uri, propfindParameters);

        if (!result.IsSuccessful)
        {
            _logger.LogError("PropFind({Uri}) was not successfull: StatusCode: {StatusCode}   Description: {Description}", uri, result.StatusCode, result.Description);
            throw new Exception($"PropFind({uri}) was not successfull: StatusCode: {result.StatusCode}   Description: {result.Description}");
        }

        foreach (var resource in result.Resources)
        {
            if (HttpUtility.UrlDecode(resource.Uri).Equals(uri, StringComparison.OrdinalIgnoreCase)
                || resource.Uri.Equals(uri, StringComparison.OrdinalIgnoreCase)) continue;
            resources.Add(resource);
        }
        return resources;
    }


    public async Task<string> GetFile(string uri, CancellationToken cancellationToken)
    {
        var fil = await GetFileFromUri(uri, cancellationToken);
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