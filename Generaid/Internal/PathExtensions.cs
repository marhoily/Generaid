using System.IO;
using System.IO.Abstractions;

namespace Generaid
{
    internal static class PathExtensions
    {
        public static void EnsureDirectoryExists(this string path, IFileSystem fs, string dir)
            => fs.Path.Combine(path, dir).EnsureDirectoryExists(fs);

        private static void EnsureDirectoryExists(this string path, IFileSystem fs)
        {
            if (fs.Directory.Exists(path)) return;
            fs.Path.GetDirectoryName(path).EnsureDirectoryExists(fs);
            fs.Directory.CreateDirectory(path);
        }
    }
}