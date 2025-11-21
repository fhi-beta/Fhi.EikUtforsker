using Microsoft.Extensions.Options;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV20;

public class KryptertRekvisisjonsmeldingV20Tjeneste(IOptions<EikUtforskerOptions> options) : EikMeldingValidatorBase(options), IMeldingsformatTjeneste
{
    protected override string SkjemaVersjon => "2.0";
    protected override EikMessageType MessageType => EikMessageType.Rekvisisjonsmelding;
    protected override ThumbprintFieldType ThumbprintField => ThumbprintFieldType.CertificateThumbprint;

    public List<string> ValiderDekryptertJson(string dekryptert) => ValiderRekvisisjonsmeldingDekryptert(dekryptert);
    public string ValiderJson(string kryptert) => ValiderKryptertRekvisisjonsmelding(kryptert);
}
