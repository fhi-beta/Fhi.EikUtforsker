using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV106
{
    public class KryptertReseptmeldingV106Tjeneste : IMeldingsformatTjeneste
    {
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;

        public KryptertReseptmeldingV106Tjeneste(IOptions<EikUtforskerOptions> options)
        {
            _storeName = (StoreName)Enum.Parse(typeof(StoreName), options.Value.StoreName);
            _storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), options.Value.StoreLocation);
        }

        public (string feilmelding, string dekryptert) Dekrypter(string kryptert)
        {
            try
            {
                var kryptertJson = JObject.Parse(kryptert);
                var keyCipherValue = JsonHelper.GetElement(kryptertJson, "kryptertReseptmelding.kryptertNokkel.keyCipherValue");
                var reseptmeldingshode = JsonHelper.GetElement(kryptertJson, "kryptertReseptmelding.reseptmeldingshode");
                var thumbprint = JsonHelper.GetElement(kryptertJson, "kryptertReseptmelding.kryptertNokkel.keyName");
                var aesKey = DekryptHelper.DekrypterLmrEikNøkkel(keyCipherValue, _storeName, _storeLocation, thumbprint);
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

        public List<string> ValiderDekryptertJson(string json)
        {
            try
            {
                return JsonSchemaHelper.GetJsonValideringsfeil(json, "reseptmelding_v106.schema.json",
                    new Dictionary<string, string>() { { "http://www.fhi.no/legemiddelregisteret/eik/reseptmelding/felles_v106.schema.json", "felles_v106.schema.json" } });
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
                return JsonSchemaHelper.ValiderJson(kryptert, "kryptertreseptmelding_v106.schema.json",
                    new Dictionary<string, string>() { { "http://www.fhi.no/legemiddelregisteret/eik/kryptertreseptmelding/felles.schema.json", "felles_v106.schema.json" } });
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
