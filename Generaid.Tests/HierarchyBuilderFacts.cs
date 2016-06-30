using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Xunit;

namespace Generaid
{
    [UseReporter(typeof(AraxisMergeReporter))]
    public sealed class HierarchyBuilderFacts
    {
        [Fact]
        public void Test()
        {
            var fs = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    ["c:/proj/sample.proj"] = new MockFileData(
                        Utils.EmbeddedResource("Generaid.sample.proj"))
                });
            var h = new HierarchyBuilder(fs, "c:/proj/sample.proj", "Generated") {
                    new NodeBuilder<ModelGenerator>(new Model()) {
                        new NodeBuilder<CompanyGenerator> {
                            new NodeBuilder<EmployeeGenerator>() } } }
                .With((Model m) => m.Companies)
                .With((Company c) => c.Employees);
            h.Generate();
            h.Generate();

            Approvals.Verify(fs.File.
                ReadAllText("c:/proj/sample.proj"));
            var generatedFiles = fs.AllFiles
                .Except(new [] { @"c:\proj\sample.proj" })
                .ToList();

            generatedFiles
                .Select(x => x.Replace(@"c:\proj\Generated\", ""))
                .Should().BeEquivalentTo("root", "Microsoft",
                "John", "Marry", "Apple", "Alice", "Bob");
            foreach (var generatedFile in generatedFiles)
            {
                fs.File.ReadAllText(generatedFile)
                    .Should()
                    .Be(generatedFile.Replace(@"c:\proj\Generated\", ""));
            }
        }

    }
}
