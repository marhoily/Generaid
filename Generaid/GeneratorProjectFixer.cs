using System;
using System.IO;
using System.Text;

namespace Generaid
{
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
        public static void FixFrojFile(string file)
        {
            var oldText = File.ReadAllText(file);
            var newText = Fix(oldText);
            if (oldText != newText)
                File.WriteAllText(file, newText);
        }

        internal static string Fix(string input)
        {
            var sb = new StringBuilder();
            var curr = 0;
            var open = input.IndexOf("<LastGenOutput>", curr, StringComparison.Ordinal);
            while (open != -1)
            {
                var close = input.IndexOf("</LastGenOutput>", open, StringComparison.Ordinal);
                if (close != -1)
                {
                    if (input.IndexOf(".g.", open, close - open, StringComparison.Ordinal) == -1)
                    {
                        var dot = input.IndexOf(".", open, close - open, StringComparison.Ordinal);
                        if (dot != -1)
                        {
                            sb.Append(input.Substring(curr, dot - curr));
                            sb.Append(".g");
                            curr = dot - curr;
                        }
                    }
                    open = input.IndexOf("<LastGenOutput>", close, StringComparison.Ordinal);
                }
                else
                {
                    open = input.IndexOf("<LastGenOutput>", open + "<LastGenOutput>".Length, StringComparison.Ordinal);
                }
            }
            sb.Append(input.Substring(curr, input.Length - curr));
            return sb.ToString();
        }
    }
}