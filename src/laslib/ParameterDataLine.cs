namespace liblas4
{
    using System.Collections.Generic;

    public class ParameterDataLine
    {
        public string Mnemonic { get; set; } = "";
        public string Value { get; set; } = "";
        public string Unit { get; set; } = "";
        public string Description  { get; set; } = "";
        public string Format { get; set; } = "";
        public List<string> assocList { get; set; } = new List<string>();
    }
}