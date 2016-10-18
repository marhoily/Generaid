using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
// ReSharper disable StringIndexOfIsCultureSpecific.3
// ReSharper disable StringIndexOfIsCultureSpecific.2

namespace GeneratedFileNamesFixer
{
    public static class Program
    {
        static void Main()
        {
            var projectFiles = Directory.EnumerateFiles(
                Environment.CurrentDirectory, "*.csproj", SearchOption.AllDirectories);
            foreach (var file in projectFiles)
            {
                Console.Write(file + "...");
                var oldText = File.ReadAllText(file);
                var newText = Fix(oldText);
                if (oldText != newText)
                {
                    File.WriteAllText(file, newText);
                    Console.Write("Fixed");
                }
                else
                {
                    Console.Write("<null>");
                }
            }

        }

        public static string Fix(string input)
        {
            var sb = new StringBuilder();
            var curr = 0;
            var open = input.IndexOf("<LastGenOutput>", curr);
            while (open != -1)
            {
                var close = input.IndexOf("</LastGenOutput>", open);
                if (close != -1)
                {
                    if (input.IndexOf(".g.", open, close - open) == -1)
                    {
                        var dot = input.IndexOf(".", open, close - open);
                        if (dot != -1)
                        {
                            sb.Append(input.Substring(curr, dot - curr));
                            sb.Append(".g");
                            curr = dot - curr;
                        }
                    }
                    open = input.IndexOf("<LastGenOutput>", close);
                }
                else
                {
                    open = input.IndexOf("<LastGenOutput>", open+ "<LastGenOutput>".Length);
                }
            }
            sb.Append(input.Substring(curr, input.Length - curr));
            return sb.ToString();
        }
    }
}
