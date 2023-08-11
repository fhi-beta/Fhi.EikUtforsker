using WebDav;

namespace Fhi.EikUtforsker.Tjenester.WebDav
{
    public class WebDavResourceNode
    {
        public WebDavResourceNode(WebDavResource resource)
        {
            Uri = resource.Uri;
            ETag = resource.ETag;
            LastModifiedDate = resource.LastModifiedDate.Value.ToUniversalTime();
            ContentLength = resource.ContentLength;
            IsCollection = resource.IsCollection;
        }

        public WebDavResourceNode(string uri, string relUri, string eTag, DateTime lastModifiedDate, long? contentLength, bool isCollection)
        {
            Uri = uri;
            ETag = eTag;
            LastModifiedDate = lastModifiedDate;
            ContentLength = contentLength;
            IsCollection = isCollection;
        }

        public string Uri { get; }
        public string Name => Uri.TrimEnd('/').Split("/").LastOrDefault() ?? "";
        public string ETag { get; }
        public DateTime LastModifiedDate { get; }
        public long? ContentLength { get; }
        public bool IsCollection { get; }
    }
}
