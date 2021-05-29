using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV099
{
    public class KryptertRekvisisjonsmeldingV099Tjeneste : IMeldingsformatTjeneste
    {
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;
        private readonly string _thumbprint;

        public KryptertRekvisisjonsmeldingV099Tjeneste(IOptions<EikUtforskerOptions> options)
        {
            _storeName = (StoreName)Enum.Parse(typeof(StoreName), options.Value.StoreName);
            _storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), options.Value.StoreLocation);
            _thumbprint = options.Value.Thumbprint;
        }

        public (string feilmelding, string dekryptert) Dekrypter(string kryptert)
        {
            var (deserialisert, errors) = JsonSerializerHelper.Deserialize<KryptertRekvisisjonsmeldingV099>(kryptert);
            if (errors.Any())
            {
                var feil = "Klarte ikke deserialsere kryptert melding: " + String.Join(", ", errors);
                return (feil, null);
            }

            try
            {
                var keyCipherValue = deserialisert?.EikApi?.KryptertRekvisisjonsmelding?.KryptertFhiNokkel?.KeyCipherValue;
                var aesKey = DekryptHelper.DekrypterNøkkel(keyCipherValue, _storeName, _storeLocation, _thumbprint);

                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        },
                    Formatting = Formatting.Indented
                };
                var rekvisisjonsmeldinghode = JsonConvert.SerializeObject(deserialisert.EikApi.KryptertRekvisisjonsmelding.Rekvisisjonsmeldinghode, serializerSettings);

                var melding = "{\n" +
                              "  \"rekvisisjonsmelding\": {\n" +
                              "    \"rekvisisjonsmeldinghode\": " + rekvisisjonsmeldinghode + "\n" +
                              "  },\n" +
                              "  \"utleveringerFraRekvisisjon\": [\n";

                int antall = deserialisert.EikApi.KryptertRekvisisjonsmelding.KryptertUtleveringFraRekvisisjon?.Length ?? 0;
                if (antall > 0)
                    foreach (var utlevering in deserialisert.EikApi.KryptertRekvisisjonsmelding.KryptertUtleveringFraRekvisisjon)
                    {
                        var identiteter = DekryptHelper.DekrypterBase64Cipher(utlevering.Rekvisisjonsidentiteter.CipherData, aesKey);
                        var informasjon = DekryptHelper.DekrypterBase64Cipher(utlevering.Rekvisisjonsinformasjon.CipherData, aesKey);

                        melding += "    {\n" +
                                   "    \"rekvisisjonsidentiteter\": " + identiteter + ",\n" +
                                   "    \"rekvisisjonsinformasjon\": " + informasjon + "\n" +
                                   "    }" + (--antall > 0 ? "," : "") + "\n";
                    }

                melding += "  ]\n" +
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

        public List<string> ValiderDekryptertJson(string json)
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

        public string ValiderJson(string kryptert)
        {
            try
            {
                return JsonSchemaHelper.ValiderJson(kryptert, "kryptertrekvisisjonsmelding_v099.schema.json");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
