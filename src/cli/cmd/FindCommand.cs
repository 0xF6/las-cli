namespace cli.cmd
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using core;
    using liblas4;

    public class FindCommand
    {
        public static int Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "las find",
                FullName = "Find las files by log-type.",
                Description = "Each all las-type files and find log-type."
            };


            app.HelpOption("-h|--help");
            var type = app.Argument("-b|--by <mnemonic>", "Set log-type pattern to search for logs file.");
            var dotnetNew = new FindCommand();
            app.OnExecute(() => dotnetNew.FindLogs(type.Value));

            try
            {
                return app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString().Color(Color.Red));
                return 1;
            }
        }

        public int FindLogs(string mnemonic)
        {
            var directory = Directory.GetCurrentDirectory();
            // я блять в душе не ебу как читать LIS файл, по этому хрен с ним
            var lasFiles = Directory
                .GetFiles(directory, "*.las", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x)); 

            if (!lasFiles.Any())
            {
                Console.WriteLine($"{":no_entry: ".Emoji()}Couldn't find a LAS files in '{directory}'..");
                return 1;
            }

            var box = new List<(FileInfo info, LASFile file)>();

            foreach (var file in lasFiles)
            {
                try
                {
                    // Да, я придумал мини-индексатор на коленке,
                    // А потом уже понял что Я ПРОСТО МОГУ НЕ ЧИТАТЬ кусок сектора с данными
                    // Но, без чтения сектора с данными - парсеру плохо, впринципе могу поправить:
                    // Но, мне впадлу ебаться с этим тухлым парсером - секс конечно круто, но нет
                    if(Indexer.HasIndexed(file))
                        box.Add((file, Indexer.ByFileName(file).las));
                    else
                    {
                        var las = LASFileReader.Open(file, true); // todo - fix parse file without '~A' sector
                        Indexer.Stage(file, las);
                        box.Add((file, las));
                    }
                }
                catch // todo trace-logging (and trace-flag)
                {
                    Console.WriteLine($"{":x: ".Emoji()}[fail] can't read '{file.Name}' in '{file.DirectoryName}'".Color(Color.DarkOrange));
                }
            }

            var result = box.Where(x => x.file.Data().logParameters.HasParameter(mnemonic));

            foreach (var (info, file) in result)
            {
                Console.WriteLine($"{":heavy_check_mark:".Emoji()} found '{mnemonic}' in '{info.FullName}'...".Color(Color.GreenYellow));
                var target = file.Data().logParameters[mnemonic];
                Console.WriteLine($"\t{target.Mnemonic} - {target.Description}, {target.Value}".Color(Color.Gray));
            }

            if (!result.Any())
            {
                Console.WriteLine($"{":no_entry: ".Emoji()}Log-type '{mnemonic}' not found in files which are located in '{directory}'..".Color(Color.DarkOrange));
                return 2;
            }

            return 0;
        }
    }
}