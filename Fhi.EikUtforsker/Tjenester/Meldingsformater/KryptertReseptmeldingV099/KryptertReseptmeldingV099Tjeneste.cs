using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV099
{
    public class KryptertReseptmeldingV099Tjeneste : IMeldingsformatTjeneste
    {
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;
        private readonly string _thumbprint;

        public KryptertReseptmeldingV099Tjeneste(IOptions<EikUtforskerOptions> options)
        {
            _storeName = (StoreName)Enum.Parse(typeof(StoreName), options.Value.StoreName);
            _storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), options.Value.StoreLocation);
            _thumbprint = options.Value.Thumbprint;
        }


        public (string feilmelding, string dekryptert) Dekrypter(string kryptert)
        {
            var (deserialisert, errors) = JsonSerializerHelper.Deserialize<KryptertReseptmeldingV099>(kryptert);
            if (errors.Any())
            {
                var feil = "Klarte ikke deserialsere kryptert melding: " + String.Join(", ", errors);
                return (feil, null);
            }
            try
            {
                var keyCipherValue = deserialisert?.EikApi?.KryptertReseptmelding?.KryptertFhiNokkel?.KeyCipherValue;
                var aesKey = DekryptHelper.DekrypterNøkkel(keyCipherValue, _storeName, _storeLocation, _thumbprint);
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                };
                var reseptmeldinghode = JsonConvert.SerializeObject(deserialisert.EikApi.KryptertReseptmelding.Reseptmeldinghode, serializerSettings);

                var melding = "{\n" +
                              "  \"reseptmelding\": {\n" +
                              "    \"reseptmeldinghode\": " + reseptmeldinghode + "\n" +
                              "  },\n";

                melding += "  \"utleveringerTilMenneske\": [\n";
                int antall = deserialisert.EikApi.KryptertReseptmelding.KryptertUtleveringTilMenneske?.Length ?? 0;
                if (antall > 0)
                    foreach (var utlevering in deserialisert.EikApi.KryptertReseptmelding.KryptertUtleveringTilMenneske)
                    {
                        var identiteter = DekryptHelper.DekrypterBase64Cipher(utlevering.UtleveringTilMenneskeIdentiteter.CipherData, aesKey);
                        var informasjon = DekryptHelper.DekrypterBase64Cipher(utlevering.UtleveringTilMenneskeInformasjon.CipherData, aesKey);
                        melding += "    {\n" +
                                   "    \"utleveringTilMenneskeIdentiteter\": " + identiteter + ",\n" +
                                   "    \"utleveringTilMenneskeInformasjon\": " + informasjon + "\n" +
                                   "    }" + (--antall > 0 ? "," : "") + "\n";
                    }
                melding += "  ],\n";

                melding += "  \"utleveringerTilDyr\": [\n";
                antall = deserialisert.EikApi.KryptertReseptmelding.KryptertUtleveringTilDyr?.Length ?? 0;
                if (antall > 0)
                    foreach (var utlevering in deserialisert.EikApi.KryptertReseptmelding.KryptertUtleveringTilDyr)
                    {
                        var identiteter = DekryptHelper.DekrypterBase64Cipher(utlevering.UtleveringTilDyrIdentiteter.CipherData, aesKey);
                        var informasjon = DekryptHelper.DekrypterBase64Cipher(utlevering.UtleveringTilDyrInformasjon.CipherData, aesKey);
                        melding += "    {\n" +
                                   "    \"utleveringTilDyrIdentiteter\": " + identiteter + ",\n" +
                                   "    \"utleveringTilDyrInformasjon\": " + informasjon + "\n" +
                                   "    }" + (--antall > 0 ? "," : "") + "\n";
                    }
                melding += "  ],\n";

                melding += "  \"utleveringerTilRekvirent\": [\n";
                antall = deserialisert.EikApi.KryptertReseptmelding.KryptertUtleveringTilRekvirent?.Length ?? 0;
                if (antall > 0)
                    foreach (var utlevering in deserialisert.EikApi.KryptertReseptmelding.KryptertUtleveringTilRekvirent)
                    {
                        var identiteter = DekryptHelper.DekrypterBase64Cipher(utlevering.UtleveringTilRekvirentIdentiteter.CipherData, aesKey);
                        var informasjon = DekryptHelper.DekrypterBase64Cipher(utlevering.UtleveringTilRekvirentInformasjon.CipherData, aesKey);
                        melding += "    {\n" +
                                   "    \"utleveringTilRekvirentIdentiteter\": " + identiteter + ",\n" +
                                   "    \"utleveringTilRekvirentInformasjon\": " + informasjon + "\n" +
                                   "    }" + (--antall > 0 ? "," : "") + "\n";
                    }
                melding += "  ]\n" +
                           "}\n";

                dynamic parsedJson = JsonConvert.DeserializeObject(melding);
                melding = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);

                return (null, melding);
            }
            catch (Exception ex)
            {
                return ("Dekryptering feilet: " + ex.Message, null);
            }
        }

        public List<string> ValiderDekryptertJson(string dekryptert)
        {
            return new List<string>();
        }

        public string ValiderJson(string kryptert)
        {
            try
            {
                return JsonSchemaHelper.ValiderJson(kryptert, "kryptertreseptmelding_v099.schema.json");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
