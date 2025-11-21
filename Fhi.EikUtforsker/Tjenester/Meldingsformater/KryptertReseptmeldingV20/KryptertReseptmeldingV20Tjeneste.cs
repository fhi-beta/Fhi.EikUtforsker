using Microsoft.Extensions.Options;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV20;

public class KryptertReseptmeldingV20Tjeneste(IOptions<EikUtforskerOptions> options)
    : EikMeldingValidatorBase(options), IMeldingsformatTjeneste
{
    protected override string SkjemaVersjon => "2.0";
    protected override EikMessageType MessageType => EikMessageType.Reseptmelding;
    protected override ThumbprintFieldType ThumbprintField => ThumbprintFieldType.CertificateThumbprint;

    public List<string> ValiderDekryptertJson(string json) => ValiderReseptmeldingDekryptert(json);
    public string ValiderJson(string kryptert) => ValiderKryptertReseptmelding(kryptert);
}
