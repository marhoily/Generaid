using System;
using System.IO;
using System.Linq;

namespace Generaid
{
    public static class Utils
    {
        public static string EmbeddedResource(string resource)
        {
            var asm = typeof(ModelGenerator).Assembly;
            var stream = asm.GetManifestResourceStream(resource);
            if (stream == null)
                throw new Exception("Following resources are available:\r\n" +
                                    string.Join("\r\n", asm.GetManifestResourceNames().Select((s, i) => $"{i}) '{s}'")));
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}