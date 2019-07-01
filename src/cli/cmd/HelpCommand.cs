namespace cli.cmd
{
    using System;
    using System.Linq;

    public class HelpCommand
    {
        // todo - auto-gen
        private const string UsageText = @"Usage: lascli [common-options] [command] [arguments]
Arguments:
  [command]     The command to execute
  [arguments]   Arguments to pass to the command
Common Options (passed before the command):
  --version     Display LAS CLI Version Number
  --info        Display LAS CLI Info
Common Commands:
  find           Find las files by log-type";

        public static int Run(string[] args)
        {
            if (args.Any())
                return Host.Main(new[] { args[0], "--help" });
            PrintHelp();
            return 0;
        }

        public static void PrintHelp()
        {
            PrintVersionHeader();
            Console.WriteLine(UsageText);
        }

        public static void PrintVersionHeader() // todo
            => Console.WriteLine("LAS-CLI v0.00 x64 []");
    }
}