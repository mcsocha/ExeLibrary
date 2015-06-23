using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace MkNewCmd
{
    public class Program
    {
        private static bool _testing = false;

        public static void Main(string[] args)
        {
            var customCmdsPathKey = ConfigurationManager.AppSettings["CustomCmdsPathKey"];
            var customCmdsPath = Environment.GetEnvironmentVariable(customCmdsPathKey, EnvironmentVariableTarget.User);
            var userprofile = Environment.GetEnvironmentVariable("USERPROFILE");
            var defaultCmdsPath = ConfigurationManager.AppSettings["DefaultCmdsPath"];
            
            if(string.IsNullOrWhiteSpace(customCmdsPath))
            {
                customCmdsPath = Path.Combine(userprofile, defaultCmdsPath);
                Environment.SetEnvironmentVariable(customCmdsPathKey, customCmdsPath, EnvironmentVariableTarget.User);
            }

            if(!_testing)
                Console.WriteLine("Enter the file name for the new cmd file w/o the '.cmd' extension:");

            var filename = !_testing ? Console.ReadLine().Trim() + ".cmd" : args[0] + ".cmd";

            var path = Path.Combine(customCmdsPath, filename);
            using(var writer = new StreamWriter(path, false))
            {
                writer.WriteLine(@"::ex path1: " + userprofile);
                writer.WriteLine(@"::ex path2: C:\Program Files (x86)");
                writer.WriteLine("start \"\" \"your path here\"");
                writer.WriteLine("exit");
            }
            
            Process.Start(new ProcessStartInfo("notepad.exe", path));
            Process.Start(new ProcessStartInfo("cmds"));
        }

        public static void Test(string filename)
        {
            _testing = true;
            Main(new string[] {filename});
        }
    }
}
