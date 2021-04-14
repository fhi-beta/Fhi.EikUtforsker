namespace Fhi.EikUtforsker.Tjenester.Meldingsformater.KrypterKonvolutt
{
    public class KryptertObjekt
    {
        public string cipherData { get; set; }
    }

    public class KryptertNokkel
    {
        public string keyCipherValue { get; set; }
        public string keyName { get; set; }
    }

    public class KrypterKonvolutt
    {
        public KryptertObjekt kryptertObjekt { get; set; }
        public KryptertNokkel kryptertNokkel { get; set; }
    }

    public class KryptertRapport
    {
        public KrypterKonvolutt kryptertKonvolutt { get; set; }
    }
}
