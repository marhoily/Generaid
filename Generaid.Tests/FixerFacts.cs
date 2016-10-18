using FluentAssertions;
using GeneratedFileNamesFixer;
using Xunit;

namespace Generaid
{
    public sealed class FixerFacts
    {
        [Fact]
        public void FactMethodName()
        {
            Program.Fix("...<LastGenOutput>WrapperInterfaceHost.cs</LastGenOutput>...")
                .Should().Be("...<LastGenOutput>WrapperInterfaceHost.g.cs</LastGenOutput>...");
        }
        [Fact]
        public void FactMethodName2()
        {
            Program.Fix("...<LastGenOutput>WrapperInterfaceHost.cs")
                .Should().Be("...<LastGenOutput>WrapperInterfaceHost.cs");
        }
        [Fact]
        public void FactMethodName1()
        {
            Program.Fix("...")
                .Should().Be("...");
        }
    }
}