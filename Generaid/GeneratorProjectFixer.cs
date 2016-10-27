using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.StringComparison;

namespace Generaid
{
    internal sealed class FixResult
    {
        public string NewText { get;  }
        public List<string> Files { get;  }

        public FixResult(string newText, List<string> files)
        {
            NewText = newText;
            Files = files;
        }
    }
    /// <summary>Contains a utility that fixes project file of the generator itself</summary>
    public static class GeneratorProjectFixer
    {
        /// <summary>
        ///     Takes a "*.csproj" file and replaces
        ///     &lt;LastGenOutput&gt; such that all *.tt templates only generate
        ///     *.g.cs files.
        /// </summary>
        /// <remarks>
        ///     You can go into resharper settings and exclude *.g.cs
        ///     files from analysis
        /// </remarks>
        public static List<string> FixProjFile(string file)
        {
            var oldText = File.ReadAllText(file);
            var result = Fix(oldText);
            if (oldText != result.NewText)
                File.WriteAllText(file, result.NewText);

            return result.Files;
        }

        internal static FixResult Fix(string input)
        {
            var files = new List<string>();
            var sb = new StringBuilder();
            var curr = 0;
            var open = input.IndexOf("<LastGenOutput>", curr, Ordinal);
            while (open != -1)
            {
                var close = input.IndexOf("</LastGenOutput>", open, Ordinal);
                if (close != -1)
                {
                    if (input.IndexOf(".g.", open, close - open, Ordinal) == -1)
                    {
                        var dot = input.IndexOf(".", open, close - open, Ordinal);
                        if (dot != -1)
                        {
                            sb.Append(input.Substring(curr, dot - curr));
                            sb.Append(".g");
                            curr = dot;
                            open += "<LastGenOutput>".Length;
                            files.Add(input.Substring(open, close-open));
                        }
                    }
                    open = input.IndexOf("<LastGenOutput>", close, Ordinal);
                }
                else
                {
                    open = input.IndexOf("<LastGenOutput>", open + "<LastGenOutput>".Length, Ordinal);
                }
            }
            sb.Append(input.Substring(curr, input.Length - curr));
            return new FixResult(sb.ToString(), files);
        }
    }
}