using FluentAssertions;
using Xunit;

namespace Generaid
{
    public sealed class FixerFacts
    {
        [Fact]
        public void FactMethodName()
        {
            Check("...<>W.cs</>...", "...<>W.g.cs</>...");
        }

        private static void Check(string actual, string expected)
        {
            GeneratorProjectFixer.Fix(Expand(actual)).Should().Be(Expand(expected));
        }

        private static string Expand(string actual)
        {
            return actual
                    .Replace("<>", "<LastGenOutput>")
                    .Replace("</>", "</LastGenOutput>");
        }

        [Fact]
        public void FactMethodName_3()
        {
            Check(
                "...<>W.g.cs</>...<>W.cs</>...",
                "...<>W.g.cs</>...<>W.g.cs</>...");
        }
        [Fact]
        public void FactMethodName_4()
        {
            Check(
                "...<>A.cs</>...<>B.cs</>...",
                "...<>A.g.cs</>...<>B.g.cs</>...");
        }
        [Fact]
        public void FactMethodName2()
        {
            Check("...<>W.cs", "...<>W.cs");
        }
        [Fact]
        public void FactMethodName1()
        {
            Check("...", "...");
        }
    }
}