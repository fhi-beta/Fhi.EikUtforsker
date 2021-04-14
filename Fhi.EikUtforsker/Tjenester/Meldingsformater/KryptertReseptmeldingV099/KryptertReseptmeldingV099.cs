using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KryptertReseptmeldingV099
{
    public class KryptertReseptmeldingV099
    {
        public EikApi EikApi { get; set; }
    }

    public class EikApi
    {
        public EikFellesInfo EikFellesInfo { get; set; }
        public KryptertReseptmelding KryptertReseptmelding { get; set; }
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

    public class KryptertReseptmelding
    {
        public KryptertNokkel KryptertFhiNokkel { get; set; }
        public KryptertNokkel KryptertTpfNokkel { get; set; }
        public KryptertUtleveringTilMenneske[] KryptertUtleveringTilMenneske { get; set; }
        public KryptertUtleveringTilDyr[] KryptertUtleveringTilDyr { get; set; }
        public KryptertUtleveringTilRekvirent[] KryptertUtleveringTilRekvirent { get; set; }
        public Reseptmeldinghode Reseptmeldinghode { get; set; }
    }

    public class KryptertNokkel
    {
        public string KeyCipherValue { get; set; }
        public string KeyName { get; set; }
    }

    public class KryptertUtleveringTilMenneske
    {
        public KryptertObjekt UtleveringTilMenneskeIdentiteter { get; set; }
        public KryptertObjekt UtleveringTilMenneskeInformasjon { get; set; }
    }

    public class KryptertUtleveringTilDyr
    {
        public KryptertObjekt UtleveringTilDyrIdentiteter { get; set; }
        public KryptertObjekt UtleveringTilDyrInformasjon { get; set; }
    }

    public class KryptertUtleveringTilRekvirent
    {
        public KryptertObjekt UtleveringTilRekvirentIdentiteter { get; set; }
        public KryptertObjekt UtleveringTilRekvirentInformasjon { get; set; }
    }

    public class KryptertObjekt
    {
        public string CipherData { get; set; }
    }

    public class Reseptmeldinghode
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
