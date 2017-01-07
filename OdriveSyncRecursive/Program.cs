using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdriveSyncRecursive
{
    class Program
    {
        private delegate Task HandleDirectory(IEnumerable<DirectoryInfo> dirs, IEnumerable<FileInfo> files);

        private static void WalkPath(DirectoryInfo basePath, HandleDirectory handler)
        {
            var paths = new Queue<DirectoryInfo>(new []{basePath});
            while (paths.Any())
            {
                var path = paths.Dequeue();
                var dirs = path.EnumerateDirectories();
                var files = path.EnumerateFiles();
                handler(dirs, files).Wait();
                foreach (var dir in path.EnumerateDirectories().OrderBy(d => d.Name))
                {
                    paths.Enqueue(dir);
                }                
            }
            
        }

        private static async Task WalkLog(IEnumerable<DirectoryInfo> dirs, IEnumerable<FileInfo> files)
        {
            await Task.Delay(0);
            foreach (var file in files)
            {
                Console.WriteLine(file.FullName);
            }
        }

        static void Main(string[] args)
        {
            WalkPath(new DirectoryInfo(@"C:\Users\accalia"), WalkLog);
        }
    }
}
