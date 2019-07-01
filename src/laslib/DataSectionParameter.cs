namespace liblas4
{
    using System.Collections.Generic;

    public class DataSectionParameter
    {
        public string Title { get; set; }

        public Dictionary<string, int> parametersMap = new Dictionary<string, int>();
        public List<ParameterDataLine> parameters = new List<ParameterDataLine>();

        /// <summary>
        /// Checks whether given mnemonic exists in the section. 
        /// </summary>
        public bool HasParameter(string mnemonic) => parametersMap.ContainsKey(mnemonic);


        public ParameterDataLine this[int index]
        {
            get
            {
                if (index < 0 || index >= parameters.Count)
                    return null;
                return parameters[index];
            }
        }
        public ParameterDataLine this[string mnemonic]
            => !HasParameter(mnemonic) ? null : parameters[parametersMap[mnemonic]];

        public void Push(ParameterDataLine param)
        {
            parameters.Add(param);
            parametersMap.Add(param.Mnemonic, parameters.Count - 1);
        }
    }
}
