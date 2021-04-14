namespace Fhi.EikUtforsker.Tjenester.Meldingsformater
{
    public class Meldingsformat
    {
        public Meldingsformat(string skjemanavn, string rotElement, string mustContain, IMeldingsformatTjeneste tjeneste, string fileExtension)
        {
            Skjemanavn = skjemanavn;
            RotElement = rotElement;
            MustContain = mustContain;
            Tjeneste = tjeneste;
            FileExtension = fileExtension;
        }

        public string RotElement { get; }
        public string Skjemanavn { get; }
        public string MustContain { get; }
        public IMeldingsformatTjeneste Tjeneste { get; }
        public string FileExtension { get; }
    }
}
