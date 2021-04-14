using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KrypterKonvolutt
{
    public class KryptertKonvoluttTjeneste : IMeldingsformatTjeneste
    {
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;
        private readonly string _thumbprint;

        public KryptertKonvoluttTjeneste(IOptions<EikUtforskerOptions> options)
        {
            _storeName = (StoreName)Enum.Parse(typeof(StoreName), options.Value.StoreName);
            _storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), options.Value.StoreLocation);
            _thumbprint = options.Value.Thumbprint;
        }

        public (string feilmelding, string dekryptert) Dekrypter(string kryptert)
        {
            var deserialisert = System.Text.Json.JsonSerializer.Deserialize<KryptertRapport>(kryptert);
            var cipherData = deserialisert?.kryptertKonvolutt?.kryptertObjekt?.cipherData;
            var keyCipherValue = deserialisert?.kryptertKonvolutt?.kryptertNokkel?.keyCipherValue;
            var aesKey = DekryptHelper.DekrypterNøkkel(keyCipherValue, _storeName, _storeLocation, _thumbprint);
            var dataAsString = DekryptHelper.DekrypterBase64Cipher(cipherData, aesKey);
            return (null, dataAsString);
        }

        public string ValiderJson(string kryptert)
        {
            try
            {
                return JsonSchemaHelper.ValiderJson(kryptert, "kryptertKonvolutt.schema.json");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
