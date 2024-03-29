﻿using System.Collections.Generic;

namespace Fhi.EikUtforsker.Tjenester.Analyse
{
    public class Dekrypteringsanalyse
    {
        public bool ErGyldigJson { get; set; }
        public string RotElement { get; set; } = "";
        public string Skjemanavn { get; set; } = "";
        public bool ErSkjemavalidert { get; set; }
        public string Skjemavalideringsfeil { get; set; } = "";
        public bool KanDekrypteres { get; set; }
        public string Dekrypteringsfeil { get; set; } = "";
        public string Thumbprint { get; set; } = "";
        public int AntallBytesDekryptert { get; set; }
        public string DekryptertFilnavn { get; set; } = "";
        public List<string> SkjemavalideringsfeilDekryptert { get; set; } = new List<string>();
    }
}