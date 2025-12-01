using Microsoft.Extensions.Options;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KrypterKonvolutt;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV099;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV099;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV105;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV106;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV105;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV106;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV107;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV107;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV20;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV20;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertFarmasoytiskTjenesteMeldingV10;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater
{
    public class Meldingsformater
    {
        private readonly IOptions<EikUtforskerOptions> _options;

        public Meldingsformat[] StøttedeFormater => new[] {
            new Meldingsformat("KryptertKonvolutt", "kryptertKonvolutt", "kryptertObjekt", new KryptertKonvoluttTjeneste(_options), "csv"),
            new Meldingsformat("KryptertReseptmeldingV0.99", "eikApi", "kryptertReseptmelding", new KryptertReseptmeldingV099Tjeneste(_options), "json"),
            new Meldingsformat("KryptertRekvisisjonsmeldingV0.99", "eikApi", "kryptertRekvisisjonsmelding", new KryptertRekvisisjonsmeldingV099Tjeneste(_options), "json"),
            new Meldingsformat("KryptertRekvisisjonsmeldingV1.05", "kryptertRekvisisjonsmelding", "\"1.05\"", new KryptertRekvisisjonsmeldingV105Tjeneste(_options), "json"),
            new Meldingsformat("KryptertReseptmeldingV1.05", "kryptertReseptmelding", "\"1.05\"", new KryptertReseptmeldingV105Tjeneste(_options), "json"),
            new Meldingsformat("KryptertRekvisisjonsmeldingV1.06", "kryptertRekvisisjonsmelding", "\"1.06\"", new KryptertRekvisisjonsmeldingV106Tjeneste(_options), "json"),
            new Meldingsformat("KryptertReseptmeldingV1.06", "kryptertReseptmelding", "\"1.06\"", new KryptertReseptmeldingV106Tjeneste(_options), "json"),
            new Meldingsformat("KryptertRekvisisjonsmeldingV1.07", "kryptertRekvisisjonsmelding", "\"1.07\"", new KryptertRekvisisjonsmeldingV107Tjeneste(_options), "json"),
            new Meldingsformat("KryptertReseptmeldingV1.07", "kryptertReseptmelding", "\"1.07\"", new KryptertReseptmeldingV107Tjeneste(_options), "json"),
            new Meldingsformat("KryptertRekvisisjonsmeldingV2.0", "kryptertRekvisisjonsmelding", "\"2.0\"", new KryptertRekvisisjonsmeldingV20Tjeneste(_options), "json"),
            new Meldingsformat("KryptertReseptmeldingV2.0", "kryptertReseptmelding", "\"2.0\"", new KryptertReseptmeldingV20Tjeneste(_options), "json"),
            new Meldingsformat("KryptertFarmasoytiskTjenesteMeldingV1.0", "kryptertFarmasoytiskTjenesteMelding", "\"1.0\"", new KryptertFarmasoytiskTjenesteMeldingV10Tjeneste(_options), "json"),
        };

        public Meldingsformater(IOptions<EikUtforskerOptions> options)
        {
            _options = options;
        }
    }
}
