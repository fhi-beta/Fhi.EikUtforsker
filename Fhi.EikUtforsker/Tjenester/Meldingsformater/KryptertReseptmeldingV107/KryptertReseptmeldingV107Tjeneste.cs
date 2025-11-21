using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV107;

public class KryptertReseptmeldingV107Tjeneste(IOptions<EikUtforskerOptions> options) : EikMeldingValidatorBase(options), IMeldingsformatTjeneste
{
    protected override string SkjemaVersjon => "1.07";
    protected override EikMessageType MessageType => EikMessageType.Reseptmelding;
    protected override ThumbprintFieldType ThumbprintField => ThumbprintFieldType.KeyName;

    public List<string> ValiderDekryptertJson(string json) => ValiderReseptmeldingDekryptert(json);
    public string ValiderJson(string kryptert) => ValiderKryptertReseptmelding(kryptert);
}
