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

        private delegate Task<bool> HandleError(Exception e);

        private static async Task WalkPath(DirectoryInfo basePath, HandleDirectory handleDirectory, HandleError errorHandler)
        {
            var paths = new Queue<DirectoryInfo>(new []{basePath});
            while (paths.Any())
            {
                var path = paths.Dequeue();
                IEnumerable<DirectoryInfo> dirs;
                IEnumerable<FileInfo> files;
                try
                {
                    dirs = path.EnumerateDirectories();
                    files = path.EnumerateFiles();
                }
                catch (Exception e)
                {
                    var fatal = await errorHandler(e);
                    if (fatal)
                    {
                        return;
                    }
                    continue;
                }
                await handleDirectory(dirs, files);
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

        private static async Task<bool> LogError(Exception e)
        {
            await Task.Delay(0);
            Console.WriteLine(e.Message);
            return false;
        }

        static void Main(string[] args)
        {
            WalkPath(new DirectoryInfo(@"C:\Users\accalia"), WalkLog, LogError).Wait();
        }
    }
}
