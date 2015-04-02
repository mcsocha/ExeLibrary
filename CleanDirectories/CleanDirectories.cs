namespace CleanDirectories
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;

    public class CleanDirectories
    {
        private static bool _testing = false;

        /// <summary>
        /// Delete all directories in a given path (args[0]), and optionally use a search pattern to filter the names of the directories to be deleted (args[1])
        /// </summary>
        /// <param name="args">ARG 1: A list of paths delimited by '|'.  ARG 2 (optional): A List of search patterns in a parallel list delimited by '|'.</param>
        public static void Main(string[] args)
        {
            try
            {
                Console.BufferWidth = Console.BufferWidth < 120 ? 120 : Console.BufferWidth;
                Console.BufferHeight = Console.BufferHeight < 300 ? 300 : Console.BufferHeight;

                if(args == null || args.Length == 0 || args.Length > 2)
                    throw new Exception(string.Format("Expected 1 or 2 parameters. {0} Provided.", args != null ? args.Length : 0));

                foreach(var arg in args)
                {
                    if(string.IsNullOrWhiteSpace(arg))
                        throw new Exception(string.Format("Parameters must not be null or whitespace."));
                }

                var delim = new char[] {'|'};
                var paths = args[0].Split(delim).ToList();
                var searchPatterns = new List<string>();
                if(args.Length == 2)
                {
                    searchPatterns = args[1].Split(delim).ToList();
                    if(searchPatterns.Count != paths.Count)
                        throw new Exception("Number of search patterns doesn't match number of paths.");
                }
                else
                    paths.ForEach(p => searchPatterns.Add("*"));

                var dirs = new List<string>();
                for(int i = 0; i < paths.Count; i++)
                {
                    if (!Directory.Exists(paths[i]))
                        throw new Exception(string.Format("Path '{0}' doesn't exist.", paths[i]));

                    var dirArr = Directory.GetDirectories(paths[i], searchPatterns[i], SearchOption.AllDirectories);


                    dirs.AddRange(dirArr);
                }
                
                foreach(var dir in dirs)
                {
                    try
                    {
                        if (Directory.Exists(dir))
                        {
                            Directory.Delete(dir, true);
                            Console.WriteLine(string.Format("{0} directory deleted.", dir));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(string.Format("Unable to delete {0}.  Access Denied.  Error: {1}", dir, e.Message));
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Path '{0}' was cleared {1}.  Press any key to continue.", 
                    args[0],
                    args.Length == 1 ? "of all folders" : string.Format("with searchPattern '{0}'", args[1]));

                if(!_testing)
                    Console.ReadKey(true);
            }
            catch(Exception e)
            {
                Console.WriteLine(string.Format("ERROR: {0}", e.Message));
            }
        }
    
        public static void Main(string[] args, bool testing)
        {
            _testing = testing;
            Main(args);
        }
    }
}