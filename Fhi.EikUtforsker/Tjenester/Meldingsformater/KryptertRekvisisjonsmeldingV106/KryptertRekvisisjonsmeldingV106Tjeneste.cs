using Microsoft.Extensions.Options;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV106;

public class KryptertRekvisisjonsmeldingV106Tjeneste(IOptions<EikUtforskerOptions> options) : EikMeldingValidatorBase(options), IMeldingsformatTjeneste
{
    protected override string SkjemaVersjon => "1.06";
    protected override EikMessageType MessageType => EikMessageType.Rekvisisjonsmelding;
    protected override ThumbprintFieldType ThumbprintField => ThumbprintFieldType.KeyName;

    public List<string> ValiderDekryptertJson(string dekryptert) => ValiderRekvisisjonsmeldingDekryptert(dekryptert);
    public string ValiderJson(string kryptert) => ValiderKryptertRekvisisjonsmelding(kryptert);
}
