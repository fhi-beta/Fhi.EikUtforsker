using System;
using WebDav;

namespace Fhi.EikUtforsker.Tjenester.WebDav
{
    public class WebDavFile
    {
        public WebDavFile(WebDavResource resource, string rootUri)
        {
            Uri = resource.Uri;
            RelUri = resource.Uri;
            if (RelUri.StartsWith(rootUri))
            {
                RelUri = RelUri.Remove(0, rootUri.Length);
                while (RelUri.StartsWith("/")) RelUri = RelUri.Remove(0, 1);
            }
            ETag = resource.ETag;
            LastModifiedDate = resource.LastModifiedDate.Value.ToUniversalTime();
            ContentLength = resource.ContentLength.Value;
            if (resource.IsCollection)
            {
                throw new Exception("Kan ikke lage en fil av en collection");
            }
        }

        public string Uri { get; }
        public string RelUri { get; }
        public string ETag { get; }
        public DateTime LastModifiedDate { get; }
        public long ContentLength { get; }
    }
}
