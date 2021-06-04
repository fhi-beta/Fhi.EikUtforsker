using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Fhi.EikUtforsker.Helpers
{
    public static class JsonHelper
    {
        public static string GetElement(JToken rot, string sti)
        {
            var path = "";
            var curr = rot;
            var elemNames = sti.Split('.');
            foreach (var elemName in elemNames)
            {
                if (path.Length > 0) path += ".";
                path += elemName;
                curr = curr[elemName] ?? throw new Exception($"Elementet {path} mangler");
            }
            return curr.ToString();
        }

        public static string Format(string melding)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(melding);
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };
            melding = JsonConvert.SerializeObject(parsedJson, serializerSettings);
            return melding;
        }
    }
}
