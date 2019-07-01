namespace cli
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using cmd;
    using Newtonsoft.Json;

    internal class Host
    {
        private static readonly Dictionary<string, Func<string[], int>> s_builtIns = new Dictionary<string, Func<string[], int>>
        {
            ["help"] = HelpCommand.Run,
            ["find"] = FindCommand.Run
        };


        public static int Main(string[] args)
        {
            InitializeProcess();
            try
            {
                return ProcessArgs(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.Color(Color.OrangeRed));
                return 1;
            }
        }


        private static void InitializeProcess()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Console.OutputEncoding = Encoding.Unicode;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore, 
                Formatting = Formatting.Indented, 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, 
                Culture = CultureInfo.InvariantCulture
            };
        }


        internal static int ProcessArgs(string[] args)
        {
            bool? verbose = null;
            var success = true;
            var command = string.Empty;
            var lastArg = 0;
            for (; lastArg < args.Length; lastArg++)
            {
                if (IsArg(args[lastArg], "h", "help") || args[lastArg] == "-?" || args[lastArg] == "/?")
                {
                    HelpCommand.PrintHelp();
                    return 0;
                }
                if (args[lastArg].StartsWith("-"))
                {
                    Console.WriteLine($"Unknown option: {args[lastArg]}");
                    success = false;
                }
                else
                {
                    command = args[lastArg];
                    break;
                }
            }
            if (!success)
            {
                HelpCommand.PrintHelp();
                return 1;
            }

            var appArgs = lastArg + 1 >= 
                          args.Length ? 
                Enumerable.Empty<string>() : 
                args.Skip(lastArg + 1).ToArray();

            if (string.IsNullOrEmpty(command))
                command = "help";

            var watch = Stopwatch.StartNew();

            int exitCode;
            if (s_builtIns.TryGetValue(command, out var builtIn))
                exitCode = builtIn(appArgs.ToArray());
            else
            {
                Console.WriteLine("Could not execute because the specified command or file was not found.".Color(Color.Red));
                exitCode = -1;
            }
                
            watch.Stop();

            Console.WriteLine($"{":sparkles:".Emoji()} Done in {watch.Elapsed.TotalSeconds:00.000}s.");

            return exitCode;
        }

        private static bool IsArg(string candidate, string longName) 
            => IsArg(candidate, null, longName);
        private static bool IsArg(string candidate, string shortName, string longName) =>
            (shortName != null && candidate.Equals($"-{shortName}", StringComparison.OrdinalIgnoreCase)) ||
            (longName != null && candidate.Equals($"--{longName}", StringComparison.OrdinalIgnoreCase));
    }
}
