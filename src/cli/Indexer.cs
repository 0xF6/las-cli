namespace cli
{
    using System;
    using System.IO;
    using System.Linq;
    using liblas4;
    using Newtonsoft.Json;

    public class Indexer
    {
        public string FullName { get; set; }
        public LASFile las { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }

        public static void Stage(FileInfo file, LASFile las)
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "las-index");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var index = new Indexer
            {
                FullName = file.FullName, 
                LastWriteTimeUtc = file.LastWriteTimeUtc, 
                las = las
            };


            File.WriteAllText(
                $"{Path.Combine(dir, $"{Guid.NewGuid()}.index.json")}", 
                JsonConvert.SerializeObject(index));
        }

        public static bool HasIndexed(FileInfo info)
            => ByFileName(info) != null;


        public static Indexer ByFileName(FileInfo file)
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "las-index");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var indexed = Directory
                .GetFiles(dir, "*.index.json")
                .Select(File.ReadAllText)
                .Select(JsonConvert.DeserializeObject<Indexer>)
                .FirstOrDefault(x => x.FullName == file.FullName);

            if (indexed is null)
                return null;
            if (indexed.LastWriteTimeUtc != file.LastWriteTimeUtc)
                return null;
            return indexed;
        }
    }
}