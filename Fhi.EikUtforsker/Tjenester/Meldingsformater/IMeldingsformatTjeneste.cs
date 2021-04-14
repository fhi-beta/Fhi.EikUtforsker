namespace Fhi.EikUtforsker.Tjenester.Meldingsformater
{
    public interface IMeldingsformatTjeneste
    {
        (string feilmelding, string dekryptert) Dekrypter(string kryptert);
        string ValiderJson(string kryptert);
    }
}
