using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;

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
            try
            {
                var kryptertJson = JObject.Parse(kryptert);
                var keyCipherValue = JsonHelper.GetElement(kryptertJson, "kryptertReseptmelding.kryptertNokkel.keyCipherValue");
                var reseptmeldingshode = JsonHelper.GetElement(kryptertJson, "kryptertReseptmelding.reseptmeldingshode");
                var aesKey = DekryptHelper.DekrypterLmrEikNøkkel(keyCipherValue, _storeName, _storeLocation, _thumbprint);
                var krypterteUtleveringer = JsonHelper.GetElement(kryptertJson, "kryptertReseptmelding.krypterteUtleveringer.cipherData");
                var utleveringer = DekryptHelper.DekrypterBase64Cipher(krypterteUtleveringer, aesKey);

                var melding = "{\n" +
                              "  \"reseptmelding\": {\n" +
                              "    \"reseptmeldingshode\": " + reseptmeldingshode + ",\n" +
                              "  \"utleveringer\": " + utleveringer + "\n" +
                              "  }\n" +
                              "}\n";

                melding = JsonHelper.Format(melding);
                return (null, melding);
            }
            catch (Exception ex)
            {
                return ("Dekryptering feilet: " + ex.Message, null);
            }
        }

        public string GetThumbprint(string kryptert)
        {
            return _thumbprint;
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
