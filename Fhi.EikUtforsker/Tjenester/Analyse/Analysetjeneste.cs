using Fhi.EikUtforsker.Tjenester.Meldingsformater;
using Fhi.EikUtforsker.Helpers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fhi.EikUtforsker.Tjenester.Analyse
{
    public class Analysetjeneste
    {
        private readonly Meldingsformater.Meldingsformater _meldingsformater;

        public Analysetjeneste(Meldingsformater.Meldingsformater meldingsformater)
        {
            _meldingsformater = meldingsformater;
        }

        public Dekrypteringsanalyse Analyser(string kryptert, string webDavUri)
        {
            var dekrypteringsanalyse = new Dekrypteringsanalyse();

            // Sjekker om filen er gyldig JSON
            dekrypteringsanalyse.ErGyldigJson = JsonSchemaHelper.ErGyldigJson(kryptert);
            if (!dekrypteringsanalyse.ErGyldigJson)
            {
                return dekrypteringsanalyse;
            }

            // Finner rot-elementet
            var rotElementRegex = new Regex("^+\\s*{\\s*\"([^\"]+)\"");
            var match = rotElementRegex.Match(kryptert);
            if (!match.Success)
            {
                dekrypteringsanalyse.ErGyldigJson = false;
                dekrypteringsanalyse.RotElement = "";
                return dekrypteringsanalyse;
            }
            dekrypteringsanalyse.RotElement = match.Groups[1].Value;

            // Finner aktuelle formater basert på rotelementet
            var aktuelleFormater = _meldingsformater.StøttedeFormater.Where(s => s.RotElement.Equals(dekrypteringsanalyse.RotElement, StringComparison.InvariantCultureIgnoreCase) && kryptert.Contains(s.MustContain, StringComparison.OrdinalIgnoreCase));
            if (!aktuelleFormater.Any())
            {
                dekrypteringsanalyse.ErSkjemavalidert = false;
                dekrypteringsanalyse.Skjemavalideringsfeil = "Ukjent skjema";
                return dekrypteringsanalyse;
            }


            // Prøver å skjema-validere mot et av de aktuelle formatene
            Meldingsformat validertFormat = null;
            foreach (var aktueltFormat in aktuelleFormater)
            {
                string valideringsfeil = aktueltFormat.Tjeneste.ValiderJson(kryptert);
                dekrypteringsanalyse.ErSkjemavalidert = valideringsfeil == null;
                dekrypteringsanalyse.Skjemavalideringsfeil = valideringsfeil;
                if (dekrypteringsanalyse.ErSkjemavalidert)
                {
                    validertFormat = aktueltFormat;
                    dekrypteringsanalyse.Skjemanavn = aktueltFormat.Skjemanavn;
                    dekrypteringsanalyse.Skjemavalideringsfeil = "";
                    break;
                }
            }
            if (!dekrypteringsanalyse.ErSkjemavalidert)
            {
                dekrypteringsanalyse.Skjemanavn = string.Join(", ", aktuelleFormater.Select(s => s.Skjemanavn));
                return dekrypteringsanalyse;
            }

            // Prøver å dekryptere
            var (feilmelding, dekryptert) = validertFormat.Tjeneste.Dekrypter(kryptert);
            dekrypteringsanalyse.KanDekrypteres = feilmelding == null;
            dekrypteringsanalyse.Dekrypteringsfeil = feilmelding ?? "";
            if (dekrypteringsanalyse.KanDekrypteres)
            {
                dekrypteringsanalyse.AntallBytesDekryptert = dekryptert.Length;
                string filnavn = $"fil.{validertFormat.FileExtension}";
                var filnavnRegex = new Regex("([^/]+)\\.\\w+$");
                var filnavnMatch = filnavnRegex.Match(webDavUri);
                if (filnavnMatch.Success)
                {
                    filnavn = $"{filnavnMatch.Groups[1].Value}.{validertFormat.FileExtension}";
                }

                dekrypteringsanalyse.DekryptertFilnavn = filnavn;
            }
            return dekrypteringsanalyse;
        }
    }
}
