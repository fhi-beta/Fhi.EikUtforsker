namespace Fhi.EikUtforsker.Tjenester.WebDav;

public class ResourcePriorityQueue
{
    private List<WebDavResourceNode> _queue = new List<WebDavResourceNode>();

    public void AddResources(List<WebDavResourceNode> resources)
    {
        _queue.AddRange(resources);
        _queue = _queue.OrderByDescending(n => n.LastModifiedDate).ToList();
    }

    public WebDavResourceNode? DequeueNewestCollectionWithHigherPriorityThan(int antall)
    {
        var pos = 0;
        foreach (var resource in _queue)
        {
            if (resource.IsCollection)
            {
                _queue.Remove(resource);
                return resource;
            }
            pos++;
            if (pos > antall) return null;
        }
        return null;
    }

    public List<WebDavResourceNode> GetNewestNonCollectionLimitTo(int antall)
    {
        var result = new List<WebDavResourceNode>();
        foreach (var resource in _queue)
        {
            if (resource.IsCollection || result.Count >= antall) return result;
            result.Add(resource);
        }
        return result;
    }
}