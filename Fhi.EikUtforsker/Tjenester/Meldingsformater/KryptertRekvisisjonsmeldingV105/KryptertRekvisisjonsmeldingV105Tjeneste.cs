using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV105
{
    public class KryptertRekvisisjonsmeldingV105Tjeneste : IMeldingsformatTjeneste
    {
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;
        private readonly string _thumbprint;

        public KryptertRekvisisjonsmeldingV105Tjeneste(IOptions<EikUtforskerOptions> options)
        {
            _storeName = (StoreName)Enum.Parse(typeof(StoreName), options.Value.StoreName);
            _storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), options.Value.StoreLocation);
            _thumbprint = options.Value.Thumbprint;
        }

        public (string feilmelding, string dekryptert) Dekrypter(string kryptert)
        {
            var (deserialisert, errors) = JsonSerializerHelper.Deserialize<KryptertRekvisisjonsmeldingV105>(kryptert);
            if (errors.Any())
            {
                var feil = "Klarte ikke deserialsere kryptert melding: " + String.Join(", ", errors);
                return (feil, null);
            }

            try
            {
                var keyCipherValue = deserialisert?.KryptertRekvisisjonsmelding?.KryptertNokkel?.KeyCipherValue;
                var aesKey = DekryptHelper.DekrypterNøkkel(keyCipherValue, _storeName, _storeLocation, _thumbprint);
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                };
                var rekvisisjonsmeldinghode = JsonConvert.SerializeObject(deserialisert.KryptertRekvisisjonsmelding.Rekvisisjonsmeldingshode, serializerSettings);
                var utleveringer = DekryptHelper.DekrypterBase64Cipher(deserialisert.KryptertRekvisisjonsmelding.KrypterteUtleveringer.CipherData, aesKey);

                var melding = "{\n" +
                              "  \"rekvisisjonsmelding\": {\n" +
                              "    \"rekvisisjonsmeldingshode\": " + rekvisisjonsmeldinghode + ",\n" +
                              "    \"utleveringer\": " + utleveringer + "\n" +
                              "  }\n" +
                              "}\n";

                dynamic parsedJson = JsonConvert.DeserializeObject(melding);
                melding = JsonConvert.SerializeObject(parsedJson, serializerSettings);

                return (null, melding);
            }
            catch (Exception ex)
            {
                return ("Dekryptering feilet: " + ex.Message, null);
            }
        }

        public List<string> ValiderDekryptertJson(string dekryptert)
        {
            return GetJsonValideringsfeil(dekryptert);
        }

        public string ValiderJson(string kryptert)
        {
            try
            {
                return JsonSchemaHelper.ValiderJson(kryptert, "kryptertrekvisisjonsmelding_v105.schema.json",
                    new Dictionary<string, string>() { { "http://www.fhi.no/legemiddelregisteret/eik/kryptertrekvisisjonsmelding/felles.schema.json", "felles_v105.schema.json" } });
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<string> GetJsonValideringsfeil(string json)
        {
            try
            {
                return JsonSchemaHelper.GetJsonValideringsfeil(json, "rekvisisjonsmelding_v105.schema.json",
                    new Dictionary<string, string>() { { "http://www.fhi.no/legemiddelregisteret/eik/rekvisisjonsmelding/felles.schema.json", "felles_v105.schema.json" } });
            }
            catch (Exception ex)
            {
                return new List<string>() { ex.Message };
            }
        }
    }
}
