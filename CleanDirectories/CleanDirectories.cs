namespace CleanDirectories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Delete all folders within a directory.  There's options available for recursive parameter "-r" (include subfolders) or top level only "-t"
    /// </summary>
    public class CleanDirectories
    {
        private class Options
        {
            public static string Recursive = "-r";
            public static string TopOnly = "-t";

            private static List<string> _all;
            public static List<string> All
            {
                get
                {
                    if(_all == null)
                        _all = new List<string>
                        {
                            Recursive,
                            TopOnly
                        };
                    return _all;
                }
            }
        }

        private static bool _testing = false;

        /// <summary>
        /// Delete all directories in a given path, and use a search pattern to filter the names of the directories to be deleted (args[1])
        /// </summary>
        /// <param name="args">
        /// args[0] = path; args[1] = searchPattern;<para/>
        /// -t search only top level directories<para/>
        /// -r search directories recursively
        /// </param>
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine();

                var width = 180;
                var height = 50;

                Console.BufferWidth = width;
                Console.BufferHeight = 1000;
                Console.WindowWidth = width;
                Console.WindowHeight = height;

                var path = args[0];
                var searchPattern = args[1];
                var searchOption = GetSearchOption(args);
                
                var dirsToDelete = new List<string>();
                if (!Directory.Exists(path))
                    throw new Exception(string.Format("Path '{0}' doesn't exist.", path));
                else
                    dirsToDelete = Directory.GetDirectories(path, searchPattern, searchOption).ToList();
                
                foreach(var dir in dirsToDelete)
                {
                    if(Directory.Exists(dir))
                    {
                        var outLine = new StringBuilder();
                        try
                        {
                            Directory.Delete(dir, true);
                            outLine.AppendFormat("{0} directory deleted.", TruncatePath(dir));
                        }
                        catch(Exception e)
                        {
                            outLine.AppendFormat("Delete for '{0}' failed.\nMessage: {1}", TruncatePath(dir), e.Message);
                        }
                        outLine.Append("\n");
                        Console.Write(outLine.ToString());
                    }                    
                }
                
                Console.Write("\n\nCleared directory: {0}\nSearch pattern: {1}\nSearch Option: {2}\n",
                    path, searchPattern, string.Format("{0}:{1}", args[2], searchOption.ToString()));
            }
            catch(Exception e)
            {
                Console.WriteLine(string.Format("ERROR: {0}", e.Message));
            }

            if (!_testing)
                PromptAnyKey();
        }

        private static string TruncatePath(string path)
        {
            var newPath = "";

            var maxLength = 50;

            var pathLength = path.Length;

            var folders = path.Split(new char[] {'/', '\\'});

            folders.Reverse().ForEachFirst(f => newPath += " < " + f, f => newPath += f.PadRight(32));

            var elipse = "...";
            if(newPath.Length > maxLength)
                newPath.Substring(0, maxLength-elipse.Length-1);

            return newPath;

        }

        private static SearchOption GetSearchOption(string[] args)
        {
            var option = default(SearchOption);

            if(args.Any(a => a == Options.Recursive))
                option = SearchOption.AllDirectories;
            else if (args.Any(a => a == Options.TopOnly))
                option = SearchOption.TopDirectoryOnly;
            else
            {
                var options = "";
                Util.GetAllStaticFieldValues<Options, string>()
                    .ForEachFirst(f =>  options += f, f => options += string.Format(", {0}", f));
                throw new Exception(string.Format("No SearchOption Supplied. ({0})", options));
            }

            return option;
        }

        private static void PromptAnyKey()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        public static void Test(string[] args)
        {
            _testing = true;
            Main(args);
        }
    }

    public static class Util
    {
        /// <summary>
        /// Invokes "action" on all but the 1st item in the enumeration.  "firstAction" is invoked only on the first item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TFirstAction"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        /// <param name="firstAction"></param>
        public static void ForEachFirst<T>(this IEnumerable<T> enumeration, Action<T> action, Action<T> firstAction)
        {
            bool firstDone = false;
            foreach (T item in enumeration)
            {
                if (firstDone)
                    action(item);
                else
                {
                    firstDone = true;
                    firstAction(item);
                }
            }
        }

        public static List<TField> GetAllStaticFieldValues<TClass, TField>(BindingFlags flags = BindingFlags.Public) where TClass : class
        {
            var vals = new List<TField>();

            typeof(TClass).GetFields(flags)
                .Where(f => f.FieldType == typeof(TField) && f.FieldType.IsAbstract)
                .ToList().ForEach(f => vals.Add((TField)f.GetValue(null)));

            return vals;
        }
    }
}