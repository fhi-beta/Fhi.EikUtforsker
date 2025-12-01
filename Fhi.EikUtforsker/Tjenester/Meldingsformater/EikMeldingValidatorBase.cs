using System.Security.Cryptography.X509Certificates;
using Fhi.EikUtforsker.Helpers;
using Fhi.Lmr.Felles.Eik;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater;

public enum EikMessageType
{
    Reseptmelding,
    Rekvisisjonsmelding,
    FarmasoytiskTjenesteMelding
}

public enum ThumbprintFieldType
{
    KeyName,              // v1.06, v1.07
    CertificateThumbprint // v2.0
}

public abstract class EikMeldingValidatorBase(IOptions<EikUtforskerOptions> options)
{
    protected readonly StoreName StoreName = (StoreName)Enum.Parse(typeof(StoreName), options.Value.StoreName);
    protected readonly StoreLocation StoreLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), options.Value.StoreLocation);

    protected abstract string SkjemaVersjon { get; }
    protected abstract EikMessageType MessageType { get; }
    protected abstract ThumbprintFieldType ThumbprintField { get; }

    // Message type helpers
    private string MessageTypeName => MessageType switch
    {
        EikMessageType.Reseptmelding => "Reseptmelding",
        EikMessageType.Rekvisisjonsmelding => "Rekvisisjonsmelding",
        EikMessageType.FarmasoytiskTjenesteMelding => "FarmasoytiskTjenesteMelding",
        _ => throw new ArgumentOutOfRangeException()
    };

    private string MessageTypeNameLower => MessageType switch
    {
        EikMessageType.Reseptmelding => "reseptmelding",
        EikMessageType.Rekvisisjonsmelding => "rekvisisjonsmelding",
        EikMessageType.FarmasoytiskTjenesteMelding => "farmasoytiskTjenesteMelding",
        _ => throw new ArgumentOutOfRangeException()
    };

    private string MessageEntryKey => MessageType switch
    {
        EikMessageType.Reseptmelding or EikMessageType.Rekvisisjonsmelding => "Utleveringer",
        EikMessageType.FarmasoytiskTjenesteMelding => "Tjenester",
        _ => throw new ArgumentOutOfRangeException()
    };
    private string MeldingshodeName => MessageType switch
    {
        EikMessageType.Reseptmelding or EikMessageType.Rekvisisjonsmelding => $"{MessageTypeNameLower}shode",
        EikMessageType.FarmasoytiskTjenesteMelding => "meldingshode",
        _ => throw new ArgumentOutOfRangeException()
    };

    private string ThumbprintFieldName => ThumbprintField == ThumbprintFieldType.KeyName ? "keyName" : "certificateThumbprint";

    public string GetThumbprint(string kryptert)
    {
        var kryptertJson = JObject.Parse(kryptert);
        var thumbprint = JsonHelper.GetElement(kryptertJson, $"kryptert{MessageTypeName}.kryptertNokkel.{ThumbprintFieldName}");
        return thumbprint;
    }

    public (string feilmelding, string dekryptert) Dekrypter(string kryptert)
    {
        try
        {
            var kryptertJson = JObject.Parse(kryptert);
            var keyCipherValue = JsonHelper.GetElement(kryptertJson, $"kryptert{MessageTypeName}.kryptertNokkel.keyCipherValue");
            var meldingshode = JsonHelper.GetElement(kryptertJson, $"kryptert{MessageTypeName}.{MeldingshodeName}");
            var thumbprint = JsonHelper.GetElement(kryptertJson, $"kryptert{MessageTypeName}.kryptertNokkel.{ThumbprintFieldName}");
            var aesKey = DekryptHelper.DekrypterLmrEikNøkkel(keyCipherValue, StoreName, StoreLocation, thumbprint);
            var krypterteUtleveringer = JsonHelper.GetElement(kryptertJson, $"kryptert{MessageTypeName}.krypterte{MessageEntryKey}.cipherData");
            var utleveringer = DekryptHelper.DekrypterBase64Cipher(krypterteUtleveringer, aesKey);

            var melding = "{\n" +
                          $"  \"{MessageTypeNameLower}\": {{\n" +
                          $"    \"{MeldingshodeName}\": " + meldingshode + ",\n" +
                          $"\"{MessageEntryKey.ToLower()}\": " + utleveringer + "\n" +
                          "  }\n" +
                          "}\n";

            melding = JsonHelper.Format(melding);
            return (string.Empty, melding);
        }
        catch (Exception ex)
        {
            return (ex.Message, string.Empty);
        }
    }

    protected List<string> ValiderReseptmeldingDekryptert(string json)
    {
        try
        {
            var validationErrors = EikMeldingSchemaValidator.ValidateEikReseptmelding(SkjemaVersjon, json);
            return [.. validationErrors.Select(e => e.ToString())];
        }
        catch (Exception ex)
        {
            return [ex.Message];
        }
    }

    protected string ValiderKryptertReseptmelding(string kryptert)
    {
        try
        {
            var feil = EikMeldingSchemaValidator.ValidateEikKryptertReseptmelding(SkjemaVersjon, kryptert);
            if (feil.Count == 0) return string.Empty;
            return string.Join(",\n", feil.Select(f => f.ToString()));
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    protected List<string> ValiderRekvisisjonsmeldingDekryptert(string json)
    {
        try
        {
            var validationErrors = EikMeldingSchemaValidator.ValidateEikRekvisisjonsmelding(SkjemaVersjon, json);
            return validationErrors.Select(e => e.ToString()).ToList();
        }
        catch (Exception ex)
        {
            return [ex.Message];
        }
    }

    protected string ValiderKryptertRekvisisjonsmelding(string kryptert)
    {
        try
        {
            var feil = EikMeldingSchemaValidator.ValidateEikKryptertRekvisisjonsmelding(SkjemaVersjon, kryptert);
            if (feil.Count == 0) return string.Empty;
            return string.Join(",\n", feil.Select(f => f.ToString()));
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    protected List<string> ValiderFarmasoytiskTjenesteMeldingDekryptert(string json)
    {
        try
        {
            var validationErrors = EikMeldingSchemaValidator.ValidateEikFarmasoytiskTjenesteMelding(SkjemaVersjon, json);
            return [.. validationErrors.Select(e => e.ToString())];
        }
        catch (Exception ex)
        {
            return [ex.Message];
        }
    }

    protected string ValiderKryptertFarmasoytiskTjenesteMelding(string kryptert)
    {
        try
        {
            var feil = EikMeldingSchemaValidator.ValidateEikKryptertFarmasoytiskTjenesteMelding(SkjemaVersjon, kryptert);
            if (feil.Count == 0) return string.Empty;
            return string.Join(",\n", feil.Select(f => f.ToString()));
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
