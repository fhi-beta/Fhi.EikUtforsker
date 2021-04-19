using System;
using System.Collections.Generic;
using WebDav;

namespace Fhi.EikUtforsker.Tjenester.WebDav
{
    public class WebDavResourceNode
    {
        public WebDavResourceNode(WebDavResource resource)
        {
            Uri = resource.Uri;
            RelUri = resource.Uri;
            ETag = resource.ETag;
            LastModifiedDate = resource.LastModifiedDate.Value.ToUniversalTime();
            ContentLength = resource.ContentLength;
            IsCollection = resource.IsCollection;
            Children = new List<WebDavResourceNode>();
        }

        public WebDavResourceNode(string uri, string relUri, string eTag, DateTime lastModifiedDate, long? contentLength, bool isCollection, List<WebDavResourceNode> children)
        {
            Uri = uri;
            RelUri = relUri;
            ETag = eTag;
            LastModifiedDate = lastModifiedDate;
            ContentLength = contentLength;
            IsCollection = isCollection;
            Children = children;
        }

        public string Uri { get; }
        public string RelUri { get; set; }
        public string ETag { get; }
        public DateTime LastModifiedDate { get; }
        public long? ContentLength { get; }
        public bool IsCollection { get; }
        public List<WebDavResourceNode> Children { get; }
    }
}
