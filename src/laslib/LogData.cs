namespace liblas4
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class LogData : IEnumerable<string[]>
    {
        /// <summary>
        /// Column Definition (Curve)
        /// </summary>
        public DataSectionParameter logDefinition = new DataSectionParameter();
        /// <summary>
        /// Parameters
        /// </summary>
        public DataSectionParameter logParameters = new DataSectionParameter();

        public List<string[]> logRecords = new List<string[]>();

        public string[] Row(int index) => logRecords[index];

        public ParameterDataLine Column(int index) => logDefinition[index];

        public int ColumnLen() => logDefinition.Count();

        public IEnumerator<string[]> GetEnumerator() => logRecords.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}