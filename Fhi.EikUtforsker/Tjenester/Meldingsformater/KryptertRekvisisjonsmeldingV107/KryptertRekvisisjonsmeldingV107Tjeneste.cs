using Microsoft.Extensions.Options;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV107;

public class KryptertRekvisisjonsmeldingV107Tjeneste(IOptions<EikUtforskerOptions> options) : EikMeldingValidatorBase(options), IMeldingsformatTjeneste
{
    protected override string SkjemaVersjon => "1.07";
    protected override EikMessageType MessageType => EikMessageType.Rekvisisjonsmelding;
    protected override ThumbprintFieldType ThumbprintField => ThumbprintFieldType.KeyName;

    public List<string> ValiderDekryptertJson(string dekryptert) => ValiderRekvisisjonsmeldingDekryptert(dekryptert);
    public string ValiderJson(string kryptert) => ValiderKryptertRekvisisjonsmelding(kryptert);
}
