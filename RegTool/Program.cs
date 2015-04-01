using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace RegTool
{
    static class Program
    {
        static StreamWriter Writer;

        static int WindowWidth = 450;
        static int WindowHeight = 450;
        static int WindowPadding = 10;

        static void Main(string[] args)
        {
            Console.BufferWidth = WindowWidth;
            Console.BufferHeight = WindowHeight;

            int textMaxWidth = WindowWidth - WindowPadding;

            var results = new List<RegEntry>();
            var errors = new List<string>();

            var outputTxtPath = Path.Combine(Environment.CurrentDirectory, @"reg values.txt");
            using (Writer = new StreamWriter(outputTxtPath))
            {
                List<RegistryKey> roots = new List<RegistryKey>
                {
                    Registry.ClassesRoot, 
                    Registry.CurrentConfig,                    
                    Registry.CurrentUser,
                    Registry.LocalMachine,
                    Registry.Users
                };

                OutputLine("Search the registry for a string:");
                OutputLine("Enter search keyword.", false);
                var token = Console.ReadLine();
                OutputLine("Search token: " + token);

                int numScanned = 0;
                foreach (var key in roots)
                    RegTool.Search(key, results, errors, ref numScanned, ref token, obj => OutputLine(obj));

                OutputLine(results.Count + "/" + numScanned + " scanned entries had matches.");

                foreach (var entry in results)
                    OutputLine(entry.Key + "\n" + entry.SubKey + "\n" + entry.Value.ToString() + "\n\n");

                AllowExit();
            }

            System.Diagnostics.Process.Start(outputTxtPath);
        }

        private static void AllowExit()
        {
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }

        private static void ShowErrors(List<string> errors)
        {
            OutputLine("The following " + errors.Count + " errors were encountered:");
            foreach (var error in errors)
                OutputLine(error);
        }      

        static void OutputLine(object line, bool toFile = true)
        {
            Console.WriteLine(line.ToString());
            if (toFile)
                Writer.WriteLine(line.ToString());
        }

        static void OutputLine(bool toFile = true)
        {
            OutputLine("", toFile);
        }
    }
}
