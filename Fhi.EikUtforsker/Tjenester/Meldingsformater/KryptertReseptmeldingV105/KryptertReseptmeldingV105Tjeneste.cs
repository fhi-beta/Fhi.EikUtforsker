using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV105
{
    public class KryptertReseptmeldingV105Tjeneste : IMeldingsformatTjeneste
    {
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;
        private readonly string _thumbprint;

        public KryptertReseptmeldingV105Tjeneste(IOptions<EikUtforskerOptions> options)
        {
            _storeName = (StoreName)Enum.Parse(typeof(StoreName), options.Value.StoreName);
            _storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), options.Value.StoreLocation);
            _thumbprint = options.Value.Thumbprint;
        }

        public (string feilmelding, string dekryptert) Dekrypter(string kryptert)
        {
            var (deserialisert, errors) = JsonSerializerHelper.Deserialize<KryptertReseptmeldingV105>(kryptert);
            if (errors.Any())
            {
                var feil = "Klarte ikke deserialsere kryptert melding: " + String.Join(", ", errors);
                return (feil, null);
            }

            try
            {
                var keyCipherValue = deserialisert?.KryptertReseptmelding?.KryptertNokkel?.KeyCipherValue;
                var aesKey = DekryptHelper.DekrypterNøkkel(keyCipherValue, _storeName, _storeLocation, _thumbprint);
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                };
                var rekvisisjonsmeldinghode = JsonConvert.SerializeObject(deserialisert.KryptertReseptmelding.Reseptmeldingshode, serializerSettings);
                var utleveringer = DekryptHelper.DekrypterBase64Cipher(deserialisert.KryptertReseptmelding.KrypterteUtleveringer.CipherData, aesKey);

                var melding = "{\n" +
                              "  \"reseptmelding\": {\n" +
                              "    \"reseptmeldingshode\": " + rekvisisjonsmeldinghode + ",\n" +
                              "  \"utleveringer\": " + utleveringer + "\n" +
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

        public List<string> ValiderDekryptertJson(string json)
        {
            try
            {
                return JsonSchemaHelper.GetJsonValideringsfeil(json, "reseptmelding_v105.schema.json",
                    new Dictionary<string, string>() { { "http://www.fhi.no/legemiddelregisteret/eik/reseptmelding/felles.schema.json", "felles_v105.schema.json" } });
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
                return JsonSchemaHelper.ValiderJson(kryptert, "kryptertreseptmelding_v105.schema.json",
                    new Dictionary<string, string>() { { "http://www.fhi.no/legemiddelregisteret/eik/kryptertreseptmelding/felles.schema.json", "felles_v105.schema.json" } });
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
