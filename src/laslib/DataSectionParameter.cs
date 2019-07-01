namespace liblas4
{
    using System.Collections;
    using System.Collections.Generic;

    public class DataSectionParameter : IEnumerable<ParameterDataLine>
    {
        public string Title { get; set; }

        private readonly Dictionary<string, int> parametersMap = new Dictionary<string, int>();
        private readonly List<ParameterDataLine> parameters = new List<ParameterDataLine>();

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
            parametersMap.Add(param.Mnemonic, parameters.Count-1);
        }

        public IEnumerator<ParameterDataLine> GetEnumerator() => parameters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
