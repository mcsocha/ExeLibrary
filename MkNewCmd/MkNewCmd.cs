using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace MkNewCmd
{
    /// <summary>
    /// Shows a cmd prompt helping you quickly create a new cmd in the CustomCmdsPath specified in the app.config. (cmd is intended to work with a keyboard shortcut util to open the prompt quickly then execute a common task)
    /// </summary>
    public class MkNewCmd
    {
        private static bool _testing = false;

        public static void Main(string[] args)
        {
            var cmdsPath = Environment.GetEnvironmentVariable("CMDS", EnvironmentVariableTarget.User);
            var userprofile = Environment.GetEnvironmentVariable("USERPROFILE", EnvironmentVariableTarget.User);
            
            if(string.IsNullOrWhiteSpace(cmdsPath))
            {
                cmdsPath = Path.Combine(userprofile, cmdsPath);
                Environment.SetEnvironmentVariable("CMDS", cmdsPath, EnvironmentVariableTarget.User);
            }

            var cmdName = "";
            if (!_testing || args.Length == 0)
            {
                Console.WriteLine("Enter the name of the cmd:");
                cmdName = Console.ReadLine().Trim();
            }
            else
            {
                cmdName = args[0];
            }

            if (File.Exists(Path.Combine(cmdsPath, string.Format("{0}.cmd", cmdName))))
            {
                Console.WriteLine("Error: cmd already exists..");
                Console.ReadLine();
                return;
            }

            if (File.Exists(Path.Combine(cmdsPath, string.Format("{0}.cmd", cmdName))))
            {
                Console.WriteLine("Error: cmd already exists..");
                Console.ReadLine();
                return;
            }


            var cmdFilename = string.Format("{0}.cmd", cmdName);
            var scriptFilename = string.Format("{0}.csx", cmdName);

            var cmdPath = Path.Combine(cmdsPath, cmdFilename);
            var scriptPath = Path.Combine(cmdsPath, scriptFilename);

            using (var writer = new StreamWriter(cmdPath, false))
            {
                writer.WriteLine(string.Format("scriptcs \"%CMDS%\\{0}.csx\" -- %1 %2 %3 %4 %5 %6 %7 %8 %9", cmdName));
                writer.WriteLine("exit");
            }
            
            using (var writer = new StreamWriter(scriptPath))
            {
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("var arg1 = Env.ScriptArgs[0];");
                writer.WriteLine("var arg2 = Env.ScriptArgs[1];");
            }

            Process.Start(new ProcessStartInfo("notepad.exe", scriptPath));
            Process.Start(new ProcessStartInfo("notepad.exe", cmdPath));
        }

        public static void Test(string filename)
        {
            _testing = true;
            Main(new string[] {filename});
        }
    }
}