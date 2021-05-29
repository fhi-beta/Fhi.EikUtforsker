using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fhi.EikUtforsker.Helpers
{
    public static class JsonSchemaHelper
    {
        private static readonly string _embededRoot = "Fhi.EikUtforsker.Meldingskjemaer";

        public static List<string> GetJsonValideringsfeil(string kryptert, string skjemafilnavn, Dictionary<string, string> underskjema = null)
        {
            var skjema = LesSkjemafil(skjemafilnavn);
            JSchema jSchema = null;

            if (underskjema == null || underskjema.Count == 0)
            {
                jSchema = JSchema.Parse(skjema);
            }
            else
            {
                var resolver = new JSchemaPreloadedResolver();
                foreach (KeyValuePair<string, string> entry in underskjema)
                {
                    var underskjemaJson = LesSkjemafil(entry.Value);
                    resolver.Add(new Uri(entry.Key), underskjemaJson);
                }
                jSchema = JSchema.Parse(skjema, resolver);
            }
            var kryptertJObject = JObject.Parse(kryptert);
            var isValid = kryptertJObject.IsValid(jSchema, out IList<string> messages);
            return messages.ToList();
        }

        public static string ValiderJson(string kryptert, string skjemafilnavn, Dictionary<string, string> underskjema=null)
        {
            var feil = GetJsonValideringsfeil(kryptert, skjemafilnavn, underskjema);
            if (!feil.Any()) return null;
            return String.Join(",\n", feil);
        }


        public static string LesSkjemafil(string skjemafilnavn)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = $"{_embededRoot}.{skjemafilnavn}";
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static bool ErGyldigJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")))
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
