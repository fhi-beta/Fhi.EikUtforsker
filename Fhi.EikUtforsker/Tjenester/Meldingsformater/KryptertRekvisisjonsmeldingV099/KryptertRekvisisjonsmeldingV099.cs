namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertRekvisisjonsmeldingV099
{
    public class KryptertRekvisisjonsmeldingV099
    {
        public EikApi EikApi { get; set; }
    }

    public class EikApi
    {
        public EikFellesInfo EikFellesInfo { get; set; }
        public KryptertRekvisisjonsmelding KryptertRekvisisjonsmelding { get; set; }
    }

    public class EikFellesInfo
    {
        public ConversationRef ConversationRef { get; set; }
        public string EikMessageTrace { get; set; }
        public string GenDate { get; set; }
        public string MsgId { get; set; }
    }

    public class ConversationRef
    {
        public string RefToConversation { get; set; }
        public string RefToParent { get; set; }
    }

    public class KryptertRekvisisjonsmelding
    {
        public KryptertNokkel KryptertFhiNokkel { get; set; }
        public KryptertNokkel KryptertTpfNokkel { get; set; }
        public KryptertUtleveringFraRekvisisjon[] KryptertUtleveringFraRekvisisjon { get; set; }
        public Rekvisisjonsmeldinghode Rekvisisjonsmeldinghode { get; set; }
    }

    public class KryptertNokkel
    {
        public string KeyCipherValue { get; set; }
        public string KeyName { get; set; }
    }

    public class KryptertUtleveringFraRekvisisjon
    {
        public KryptertObjekt Rekvisisjonsidentiteter { get; set; }
        public KryptertObjekt Rekvisisjonsinformasjon { get; set; }
    }

    public class KryptertObjekt
    {
        public string CipherData { get; set; }
    }

    public class Rekvisisjonsmeldinghode
    {
        public int AntallUtleveringer { get; set; }
        public AvsenderSystem GenerertAv { get; set; }
        public string Konsesjonsnummer { get; set; }
        public string TidspunktGenerert { get; set; }
        public string[] Utleveringsdatoer { get; set; }
    }

    public class AvsenderSystem
    {
        public string Systemleverandor { get; set; }
        public string Systemnavn { get; set; }
        public string Systemversjon { get; set; }
    }
}
