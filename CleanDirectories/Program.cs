using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CleanDirectories
{
    class Program
    {
        /// <summary>
        /// Delete all directories in a given path (args[0]), and optionally use a search pattern to filter the names of the directories to be deleted (args[1])
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Console.BufferWidth = Console.BufferWidth < 120 ? 120 : Console.BufferWidth;
                Console.BufferHeight = Console.BufferHeight < 300 ? 300 : Console.BufferHeight;

                if(args == null || args.Length == 0 || args.Length > 2)
                    throw new Exception(string.Format("Expected 1 or 2 parameters. {0} Provided.", args != null ? args.Length : 0));

                var path = args[0];
                var searchPattern = args.Length == 2 ? args[1] : "*";

                if(!Directory.Exists(args[0]))
                    throw new Exception(string.Format("Path '{0}' doesn't exist."));

            
                foreach(var dir in Directory.EnumerateDirectories(path, searchPattern, SearchOption.AllDirectories))
                {
                    try
                    {
                        if(Directory.Exists(dir))
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

                Console.WriteLine("Path '{0}' was cleared {1}.  Press any key to continue.", 
                    path,
                    searchPattern == "*" ? "of all folders" : string.Format("of all folders matching '{0}'"));

                Console.ReadKey(true);
            }
            catch(Exception e)
            {
                Console.WriteLine(string.Format("ERROR: {0}", e.Message));
            }
        }
    }
}