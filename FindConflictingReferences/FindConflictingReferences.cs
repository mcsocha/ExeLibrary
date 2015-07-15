using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace FindConflictingReferences
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Read/write a registry key
    /// </summary>
    public class FindConflictingReferences
    {
        public static void Main(string[] args)
        {
            var width = 150;
            var height = 50;
            Console.WindowHeight = height;
            Console.WindowWidth = width;
            Console.BufferHeight = height * 10;
            Console.BufferWidth = width;

            var path = @"C:\Code\mlmfp1-85-svn\mlm\trunk\UW2008\bin\Debug";
            var assemblies = GetAllAssemblies(path);

            var references = GetReferencesFromAllAssemblies(assemblies);
 
            var groupsOfConflicts = FindReferencesWithTheSameShortNameButDiffererentFullNames(references);

            Console.WriteLine(string.Format("Finding conflicting references for dll's and exe's in {0}", path));
            Console.WriteLine();
            Console.WriteLine();
 
            foreach (var group in groupsOfConflicts)
            {
                Console.WriteLine("Possible conflicts for {0}:", group.Key);
                Console.WriteLine();
                foreach (var reference in group)
                {
                    var ass = reference.Assembly;
                    var refAss = reference.ReferencedAssembly;
                    Console.WriteLine("{0} refs>>     {1}",
                                          string.Format("{0} ({1})", ass.Name, ass.Version).PadRight(65),
                                          string.Format("{0} ({1})", refAss.Name, refAss.Version).PadRight(40));
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine("Press escape to exit.");
            ConsoleKeyInfo key = default(ConsoleKeyInfo);
            do
            {
                key = Console.ReadKey(true);
            }
            while(key.Key != ConsoleKey.Escape);
        }
 
        private static IEnumerable<IGrouping<string, Reference>> FindReferencesWithTheSameShortNameButDiffererentFullNames(List<Reference> references)
        {
            return from reference in references
                   group reference by reference.ReferencedAssembly.Name
                       into referenceGroup
                       where referenceGroup.ToList().Select(reference => reference.ReferencedAssembly.FullName).Distinct().Count() > 1
                       select referenceGroup;
        }
 
        private static List<Reference> GetReferencesFromAllAssemblies(List<Assembly> assemblies)
        {
            var references = new List<Reference>();
            foreach (var assembly in assemblies)
            {
                foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                {
                    references.Add(new Reference
                    {
                        Assembly = assembly.GetName(),
                        ReferencedAssembly = referencedAssembly
                    });
                }
            }
            return references;
        }
 
        private static List<Assembly> GetAllAssemblies(string path)
        {
            var files = new List<FileInfo>();
            var directoryToSearch = new DirectoryInfo(path);
            files.AddRange(directoryToSearch.GetFiles("*.dll", SearchOption.AllDirectories));
            files.AddRange(directoryToSearch.GetFiles("*.exe", SearchOption.AllDirectories));
            return files.ConvertAll(file => Assembly.LoadFile(file.FullName));
        }
 
        private class Reference
        {
            public AssemblyName Assembly { get; set; }
            public AssemblyName ReferencedAssembly { get; set; }
        } 
    }
}