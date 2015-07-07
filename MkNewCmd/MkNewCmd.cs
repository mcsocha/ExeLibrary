﻿using System;
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
                writer.WriteLine("start \"\" \"\"");
                writer.WriteLine("exit");
            }

            Process.Start(new ProcessStartInfo("explorer.exe", customCmdsPath));
            Process.Start(new ProcessStartInfo("notepad.exe", path));
        }

        public static void Test(string filename)
        {
            _testing = true;
            Main(new string[] {filename});
        }
    }
}