using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;

namespace Fhi.EikUtforsker.Helpers
{
    public static class JsonSerializerHelper
    {
        public static (T, IList<string>) Deserialize<T>(string kryptert) where T : class
        {
            var errors = new List<string>();

            var serializer = new JsonSerializer();
            serializer.Error += delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                if (args.CurrentObject == args.ErrorContext.OriginalObject)
                {
                    errors.Add(args.ErrorContext.Error.Message);
                }
            };
            var reader = new JsonTextReader(new StringReader(kryptert));
            T deserialisert = null;
            try
            {
                deserialisert = serializer.Deserialize<T>(reader);
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            return (deserialisert, errors);
        }
    }
}
