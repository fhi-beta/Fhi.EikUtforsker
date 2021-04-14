using System;
using System.Linq;

namespace Fhi.EikUtforsker.Tjenester.Dekryptering
{
    public class Dekrypteringstjeneste
    {
        private readonly Meldingsformater.Meldingsformater _meldingsformater;
        public Dekrypteringstjeneste(Meldingsformater.Meldingsformater meldingsformater)
        {
            _meldingsformater = meldingsformater;
        }

        public (string feilmelding, string dekryptert) Dekrypter(string kryptert, string skjemanavn)
        {
            var skjema = _meldingsformater.StøttedeFormater.FirstOrDefault(s => s.Skjemanavn.Equals(skjemanavn, StringComparison.OrdinalIgnoreCase));

            if (skjema == null)
            {
                return ($"Ukjent skjemanavn {skjemanavn}", null);
            }
            return skjema.Tjeneste.Dekrypter(kryptert);
        }
    }
}
