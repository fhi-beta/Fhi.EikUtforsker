using Microsoft.Extensions.Options;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KrypterKonvolutt;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV099;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV099;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV105;
using Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV105;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater
{
    public class Meldingsformater
    {
        private readonly IOptions<EikUtforskerOptions> _options;

        public Meldingsformat[] StøttedeFormater => new[] {
            new Meldingsformat("KryptertKonvolutt", "kryptertKonvolutt", "kryptertObjekt", new KryptertKonvoluttTjeneste(_options), "csv"),
            new Meldingsformat("KryptertReseptmeldingV0.99", "eikApi", "kryptertReseptmelding", new KryptertReseptmeldingV099Tjeneste(_options), "json"),
            new Meldingsformat("KryptertRekvisisjonsmeldingV0.99", "eikApi", "kryptertRekvisisjonsmelding", new KryptertRekvisisjonsmeldingV099Tjeneste(_options), "json"),
            new Meldingsformat("KryptertRekvisisjonsmeldingV1.05", "kryptertRekvisisjonsmelding", "rekvisisjonsmeldingshode", new KryptertRekvisisjonsmeldingV105Tjeneste(_options), "json"),
            new Meldingsformat("KryptertReseptmeldingV1.05", "kryptertReseptmelding", "reseptmeldingshode", new KryptertReseptmeldingV105Tjeneste(_options), "json")
        };

        public Meldingsformater(IOptions<EikUtforskerOptions> options)
        {
            _options = options;
        }
    }
}
