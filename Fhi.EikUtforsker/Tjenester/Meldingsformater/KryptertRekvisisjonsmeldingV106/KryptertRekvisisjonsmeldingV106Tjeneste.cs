﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV106
{
    public class KryptertRekvisisjonsmeldingV106Tjeneste : IMeldingsformatTjeneste
    {
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;
        private readonly string _thumbprint;

        public KryptertRekvisisjonsmeldingV106Tjeneste(IOptions<EikUtforskerOptions> options)
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
                var keyCipherValue = JsonHelper.GetElement(kryptertJson, "kryptertRekvisisjonsmelding.kryptertNokkel.keyCipherValue");
                var rekvisisjonsmeldingshode = JsonHelper.GetElement(kryptertJson, "kryptertRekvisisjonsmelding.rekvisisjonsmeldingshode");
                var aesKey = DekryptHelper.DekrypterLmrEikNøkkel(keyCipherValue, _storeName, _storeLocation, _thumbprint);
                var krypterteUtleveringer = JsonHelper.GetElement(kryptertJson, "kryptertRekvisisjonsmelding.krypterteUtleveringer.cipherData");
                var utleveringer = DekryptHelper.DekrypterBase64Cipher(krypterteUtleveringer, aesKey);

                var melding = "{\n" +
                              "  \"rekvisisjonsmelding\": {\n" +
                              "    \"rekvisisjonsmeldingshode\": " + rekvisisjonsmeldingshode + ",\n" +
                              "    \"utleveringer\": " + utleveringer + "\n" +
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

        public List<string> ValiderDekryptertJson(string dekryptert)
        {
            return GetJsonValideringsfeil(dekryptert);
        }

        public string ValiderJson(string kryptert)
        {
            try
            {
                return JsonSchemaHelper.ValiderJson(kryptert, "kryptertrekvisisjonsmelding_v106.schema.json",
                    new Dictionary<string, string>() { { "http://www.fhi.no/legemiddelregisteret/eik/kryptertrekvisisjonsmelding/felles.schema.json", "felles_v106.schema.json" } });
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
                return JsonSchemaHelper.GetJsonValideringsfeil(json, "rekvisisjonsmelding_v106.schema.json",
                    new Dictionary<string, string>() { { "http://www.fhi.no/legemiddelregisteret/eik/rekvisisjonsmelding/felles.schema.json", "felles_v106.schema.json" } });
            }
            catch (Exception ex)
            {
                return new List<string>() { ex.Message };
            }
        }
    }
}
