using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Generaid
{
    public sealed class FixerFacts
    {
        [Fact]
        public void One_Fix()
        {
            Check("...<>W.cs</>...", "...<>W.g.cs</>...")
                .Should().Equal("W.cs");
        }
        [Fact]
        public void One_Fix_One_AlreadyFixed()
        {
            Check(
                "...<>X.g.cs</>...<>W.cs</>...",
                "...<>X.g.cs</>...<>W.g.cs</>...")
                .Should().Equal("W.cs");

        }
        [Fact]
        public void Two_Fixes()
        {
            Check(
                "...<>A.cs</>...<>B.cs</>...",
                "...<>A.g.cs</>...<>B.g.cs</>...")
                .Should().Equal("A.cs", "B.cs"); 
        }
        [Fact]
        public void No_Close_Tag()
        {
            Check("...<>W.cs", "...<>W.cs")
                .Should().BeEmpty();
        }
        [Fact]
        public void No_Tags()
        {
            Check("...", "...").Should().BeEmpty();
        }

        private static List<string> Check(string actual, string expected)
        {
            var fixResult = GeneratorProjectFixer.Fix(Expand(actual));
            fixResult.NewText.Should().Be(Expand(expected));
            return fixResult.Files;
        }
        private static string Expand(string actual)
        {
            return actual
                    .Replace("<>", "<LastGenOutput>")
                    .Replace("</>", "</LastGenOutput>");
        }
    }
}