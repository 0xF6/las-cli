namespace liblas4
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class LogData
    {
        /// <summary>
        /// Column Definition (Curve)
        /// </summary>
        public DataSectionParameter logDefinition = new DataSectionParameter();
        /// <summary>
        /// Parameters
        /// </summary>
        public DataSectionParameter logParameters = new DataSectionParameter();

        [JsonIgnore]
        public List<string[]> logRecords = new List<string[]>();

        public int ColumnLen() => logDefinition.parametersMap.Count;
    }
}