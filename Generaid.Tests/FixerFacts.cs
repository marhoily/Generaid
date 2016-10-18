using FluentAssertions;
using Xunit;

namespace Generaid
{
    public sealed class FixerFacts
    {
        [Fact]
        public void FactMethodName()
        {
            GeneratorProjectFixer.Fix("...<LastGenOutput>WrapperInterfaceHost.cs</LastGenOutput>...")
                .Should().Be("...<LastGenOutput>WrapperInterfaceHost.g.cs</LastGenOutput>...");
        }
        [Fact]
        public void FactMethodName_3()
        {
            GeneratorProjectFixer.Fix("...<LastGenOutput>WrapperInterfaceHost.g.cs</LastGenOutput>...<LastGenOutput>WrapperInterfaceHost.cs</LastGenOutput>...")
                .Should().Be("...<LastGenOutput>WrapperInterfaceHost.g.cs</LastGenOutput>...<LastGenOutput>WrapperInterfaceHost.g.cs</LastGenOutput>...");
        }
        [Fact]
        public void FactMethodName2()
        {
            GeneratorProjectFixer.Fix("...<LastGenOutput>WrapperInterfaceHost.cs")
                .Should().Be("...<LastGenOutput>WrapperInterfaceHost.cs");
        }
        [Fact]
        public void FactMethodName1()
        {
            GeneratorProjectFixer.Fix("...")
                .Should().Be("...");
        }
    }
}