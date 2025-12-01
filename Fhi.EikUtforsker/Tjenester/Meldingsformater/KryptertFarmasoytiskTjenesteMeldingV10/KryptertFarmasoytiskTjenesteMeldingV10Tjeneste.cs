using Fhi.EikUtforsker.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertFarmasoytiskTjenesteMeldingV10;

public class KryptertFarmasoytiskTjenesteMeldingV10Tjeneste(IOptions<EikUtforskerOptions> options)
    : EikMeldingValidatorBase(options), IMeldingsformatTjeneste
{
    protected override string SkjemaVersjon => "1.0";
    protected override EikMessageType MessageType => EikMessageType.FarmasoytiskTjenesteMelding;
    protected override ThumbprintFieldType ThumbprintField => ThumbprintFieldType.CertificateThumbprint;

    public List<string> ValiderDekryptertJson(string json) => ValiderFarmasoytiskTjenesteMeldingDekryptert(json);
    public string ValiderJson(string kryptert) => ValiderKryptertFarmasoytiskTjenesteMelding(kryptert);
}
