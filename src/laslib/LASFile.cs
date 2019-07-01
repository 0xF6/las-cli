namespace liblas4
{
    using System.Collections.Generic;

    public class LASFile
    {
        public DataSectionParameter versionSection { get; set; }
        public DataSectionParameter wellSection { get; set; }
        public Dictionary<string, LogData> data = new Dictionary<string, LogData>();
        public DataSectionParameter other_section;
        public LogData Data(string t) => data[t];
        public LogData Data() => data["Log"];

        public LASFileReader.LASVersion Version { get; set; }
    }
}