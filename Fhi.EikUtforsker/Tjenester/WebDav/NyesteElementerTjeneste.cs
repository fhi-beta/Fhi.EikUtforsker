using System.IO;
using WebDav;

namespace Fhi.EikUtforsker.Tjenester.WebDav
{
    public class NyesteElementerTjeneste
    {
        private readonly WebDavTjeneste _webDavTjeneste;

        public NyesteElementerTjeneste(WebDavTjeneste webDavTjeneste)
        {
            _webDavTjeneste = webDavTjeneste;
        }

        public async Task<List<WebDavResourceNode>> HentDeNyeste(int antall, string uri)
        {
            var queue = new ResourcePriorityQueue();

            var resources = await _webDavTjeneste.GetResources(uri);
            queue.AddResources(resources);

            while (true)
            {
                var resource = queue.DequeueNewestCollectionWithHigherPriorityThan(antall);
                if (resource == null) break;

                var nivå = resource.Uri.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length;

                var applyTo = ApplyTo.Propfind.ResourceAndChildren;
                if (nivå > 3) applyTo = ApplyTo.Propfind.ResourceAndAncestors;
                resources = await _webDavTjeneste.GetResources(resource.Uri, applyTo);
                queue.AddResources(resources);
            }
            resources = queue.GetNewestNonCollectionLimitTo(antall);
            return resources;
        }
    }
}
