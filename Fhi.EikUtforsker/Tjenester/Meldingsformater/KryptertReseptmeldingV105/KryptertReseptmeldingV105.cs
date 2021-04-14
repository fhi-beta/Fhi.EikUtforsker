namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV105
{
    public class KryptertReseptmeldingV105
    {
        public KryptertReseptmelding KryptertReseptmelding { get; set; }
    }


    public class KryptertReseptmelding
    {
        public Reseptmeldingshode Reseptmeldingshode { get; set; }
        public KryptertNokkel KryptertNokkel { get; set; }
        public KryptertObjekt KrypterteUtleveringer { get; set; }
    }

    public class KryptertNokkel
    {
        public string KeyName { get; set; }
        public string KeyCipherValue { get; set; }
    }

    public class KryptertObjekt
    {
        public string CipherData { get; set; }
    }

    public class Reseptmeldingshode
    {
        public string MeldingsId { get; set; }
        public int AntallUtleveringer { get; set; }
        public string Konsesjonsnummer { get; set; }
        public string[] Utleveringsdatoer { get; set; }
        public string TidspunktGenerert { get; set; }
        public System GenerertAv { get; set; }
        public string Skjemaversjonsnummer { get; set; }
    }

    public class System
    {
        public string Systemleverandor { get; set; }
        public string Systemnavn { get; set; }
        public string Systemversjon { get; set; }
    }
}
