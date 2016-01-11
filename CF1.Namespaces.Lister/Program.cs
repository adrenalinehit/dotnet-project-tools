using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace CF1.Namespaces.Lister
{
    /// <summary>
    /// Simple namespace lister when given a list of directories potentially containing assemblies with namespaces
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("You'll need to provide some directories here!", nameof(args));
            }

            foreach (var path in args)
            {
                if (File.Exists(path))
                {
                    // This path is a file
                    ProcessFile(path);
                }
                else if (Directory.Exists(path))
                {
                    // This path is a directory
                    ProcessDirectory(path);
                }
                else
                {
                    throw new ArgumentException($"{path} is not a valid file or directory.", nameof(path));
                }
            }

            Console.WriteLine("Finished! Press <ENTER> to exit.");
            Console.ReadLine();

        }

        //Borrowed from MS website - https://msdn.microsoft.com/en-us/library/07wt70x2(v=vs.110).aspx
        private static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory, "*.dll");
            foreach (var fileName in fileEntries)
            {
                ProcessFile(fileName);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (var subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory);
            }
        }

        private static void ProcessFile(string path)
        {
            try
            {
                var assemblyFileInfo = new FileInfo(path); //be able to just get the file name regardless of path
                var theAssembly = Assembly.LoadFrom(path);
                foreach (var nameSpace in GetNamespaces(theAssembly))
                {
                    Console.WriteLine(assemblyFileInfo.Name + ", " + nameSpace);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        //Credit: http://stackoverflow.com/questions/9188203/get-list-of-namespaces-available-in-project-in-asp-net
        private static IEnumerable<string> GetNamespaces(Assembly assembly)
        {
            return assembly.GetTypes().Select(type => type.Namespace).Distinct();
        }
    }
}
