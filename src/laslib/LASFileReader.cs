namespace liblas4
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class LASFileReader
    {
        public static LASFile Open(FileInfo file)
        {
            var lines = File.ReadAllLines(file.FullName);
            var sectors = new List<List<string>>();
            var last = -1;
            var i = 0;
            for (i = 0; i != lines.Length; i++)
            {
                if (!lines[i].StartsWith("~")) 
                    continue;
                if (last >= 0) 
                    sectors.Add(lines.ToList().GetRange( last, i - last));
                last = i;
            }
            sectors.Add(lines.ToList().GetRange(last, i - last));
            if (CheckVersion(sectors) != LASVersion.v2)
                throw new InvalidOperationException("las file is corrupted.");
            return open2_0(sectors);
        }

        private static LASFile open2_0(IReadOnlyList<List<string>> sections)
        {
            var lasFile = new LASFile
            {
                Version = LASVersion.v2
            };
            var indexV = -1;
            var indexW = -1;
            var indexC = -1;
            var indexP = -1;
            var indexO = -1;
            var indexA = -1;
            for (var i = 0; i < sections.Count; i++)
            {
                var sectionTitle = sections[i][0].Trim();
                if (sectionTitle.StartsWith("~V"))
                    indexV = i;
                else if (sectionTitle.StartsWith("~W"))
                    indexW = i;
                else if (sectionTitle.StartsWith("~C"))
                    indexC = i;
                else if (sectionTitle.StartsWith("~P"))
                    indexP = i;
                else if (sectionTitle.StartsWith("~O"))
                    indexO = i;
                else if (sectionTitle.StartsWith("~A")) 
                    indexA = i;
            }
            if (indexV == -1)
                throw new Exception("Missing section ~V");
            lasFile.versionSection = BuildParameterDataSection(sections[indexV]);
            // Well
            if (indexW == -1)
                throw new Exception("Missing section ~W");
            lasFile.wellSection = BuildParameterDataSection(sections[indexW]);
            // Other
            lasFile.other_section = indexO != -1 ? 
                BuildParameterDataSection(sections[indexO]) : 
                new DataSectionParameter();
            // Curve
            if (indexC == -1)
                throw new Exception("Missing section ~C");
            var curve = BuildParameterDataSection(sections[indexC]);
            // Parameters
            var parameters = indexP != -1 ? 
                BuildParameterDataSection(sections[indexP]) : 
                new DataSectionParameter();
            // ASCII
            if (indexA == -1)
                throw new Exception("Missing section ~A");
            lasFile.data.Add("Log", BuildData(curve, parameters, sections[indexA]));
            // identifying wrap mode
            if (!lasFile.versionSection.HasParameter("WRAP"))
                throw new Exception("Missing parameter: WRAP.");
            switch (lasFile.versionSection["WRAP"].Value)
            {
                case "YES": break;
                case "NO" : break;
                default:
                    throw new Exception("Invalid parameter value for WRAP.");
            }
            return lasFile;
        }

        public enum LASVersion
        {
            Unk,
            v2,
            v3
        }

        protected static LogData BuildData(DataSectionParameter curveInfo, DataSectionParameter parameters, List<string> ascii)
        {
            var data = new LogData
            {
                logDefinition = curveInfo,
                logParameters = parameters
            };
            var colsRead = 0;
            var numCols = data.ColumnLen();
            var record = new string[numCols];
            ascii.RemoveAt(0);
            foreach (var line2 in ascii)
            {
                if (colsRead == numCols)
                {
                    record = new string[numCols];
                    colsRead = 0;
                }
                var line = line2.Trim();
                var array = line.Split(' ', '+');
                foreach (var value in array)
                {
                    if (colsRead >= numCols)
                        continue; //throw new Exception("Could not parse data: column number does not match.");
                    record[colsRead] = value;
                    colsRead += 1;
                }
                if (colsRead == numCols) data.logRecords.Add(record);
            }
            return data;
        }
        protected static DataSectionParameter BuildParameterDataSection(List<string> section)
        {
            var thisSection = new DataSectionParameter();
            var title = section[0].Trim();
            thisSection.Title = title;
            foreach (var line2 in section)
            {
                var line = line2.Trim();
                if (line.StartsWith("#") || line.StartsWith("~"))
                    continue;
                var parameterDataLine = BuildParameterDataLine(line);
                thisSection.Push(parameterDataLine);
            }
            return thisSection;
        }
        private static LASVersion CheckVersion(IReadOnlyList<List<string>> sections)
        {
            var i = 0;
            while (i < sections.Count)
            {
                var currentSection = sections[i];
                var sectionTitle = currentSection[0].Trim().ToUpper(CultureInfo.InvariantCulture);
                if (sectionTitle.StartsWith("~V"))
                {
                    foreach (var line2 in currentSection)
                    {
                        var line = line2.Trim();
                        if (line.StartsWith("~") || line.StartsWith("#"))
                            continue;
                        try
                        {
                            var pd = BuildParameterDataLine(line);

                            if (!pd.Mnemonic.Equals("VERS"))
                                continue;
                            if (pd.Value.StartsWith("2.0"))
                                return LASVersion.v2;
                            if (pd.Value.StartsWith("3.0"))
                                return LASVersion.v3;
                        }
                        catch { }
                    }
                }
                i++;
            }
            return LASVersion.Unk;
        }

        protected static ParameterDataLine BuildParameterDataLine(string line)
        {
            // MNEM.UNIT   VALUE : DESCRIPTION
            var p = new ParameterDataLine();
            var idxDot = line.IndexOf('.');
            idxDot = idxDot < 0 ? 0 : idxDot;
            var idxVal = line.IndexOf(' ', idxDot + 1);
            idxVal = idxVal < idxDot + 1 ? idxDot + 1 : idxVal;
            var idxDes = line.IndexOf(':', idxVal + 1);
            //idxDes = idxDes < idxVal + 1 ? line.Length - 1 : idxDes;

            p.Mnemonic = line.Substring(0, idxDot).Trim();
            p.Unit = line.Substring(idxDot + 1, idxVal).Trim();
            p.Value = line.Substring(idxVal, idxDes - 3).Trim().Trim(':');
            p.Description = line.Substring(idxDes, line.Length - idxDes).Trim().Trim(':').TrimStart(' ');
            return p;
        }
    }
}