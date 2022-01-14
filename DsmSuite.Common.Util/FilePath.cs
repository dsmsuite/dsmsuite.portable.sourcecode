using System;
using System.Collections.Generic;
using System.IO;

namespace DsmSuite.Common.Util
{
    public static class FilePath
    {
        public static string ResolveRelativePath(string relativePath)
        {
            string pathExecutingAssembly = AppDomain.CurrentDomain.BaseDirectory;
            string[] paths = { pathExecutingAssembly, relativePath };
            return new FileInfo(Path.Combine(paths)).FullName;
        }
        
        public static string ResolveFile(string path, string filename)
        {
            return Resolve(path, filename);
        }

        public static List<string> ResolveFiles(string path, IEnumerable<string> filenames)
        {
            List < string > resolvedFiles = new List<string>();
            foreach(string filename in filenames)
            {
                resolvedFiles.Add(Resolve(path, filename));
            }

            return resolvedFiles;
        }

        private static string Resolve(string path, string filename)
        {
            string absoluteFilename = Path.GetFullPath(filename);
            if (!File.Exists(absoluteFilename))
            {
                absoluteFilename = Path.GetFullPath(Path.Combine(path, filename));
            }

            Logger.LogInfo("Resolve file: path=" + path + " file=" + filename + " as file=" + absoluteFilename);
            return absoluteFilename;
        }
    }
}
