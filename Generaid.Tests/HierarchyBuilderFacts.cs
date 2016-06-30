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
        private readonly MockFileSystem _fs;
        private readonly HierarchyBuilder _hierarchyBuilder;

        public HierarchyBuilderFacts()
        {
            _fs = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    ["c:/proj/sample.proj"] = new MockFileData(
                        Utils.EmbeddedResource("Generaid.sample.proj"))
                });
            _hierarchyBuilder = new HierarchyBuilder(_fs, "c:/proj/sample.proj", "Generated") {
                new NodeBuilder<ModelGenerator>(new Model()) {
                    new NodeBuilder<CompanyGenerator> {
                        new NodeBuilder<EmployeeGenerator>() } } }
                .With((Model m) => m.Companies)
                .With((Company c) => c.Employees);
        }

        [Fact]
        public void Double_Generate_Doesnt_Fail()
        {
            _hierarchyBuilder.Generate();
            _hierarchyBuilder.Generate();
        }

        [Fact]
        public void Generate_Changes_Project_File()
        {
            _hierarchyBuilder.Generate();

            Approvals.Verify(_fs.File.
                ReadAllText("c:/proj/sample.proj"));
        }
        [Fact]
        public void Generate_Creates_Transformed_Files()
        {
            _hierarchyBuilder.Generate();

            var generatedFiles = _fs.AllFiles
                .Except(new [] { @"c:\proj\sample.proj" })
                .ToList();

            generatedFiles
                .Select(x => x.Replace(@"c:\proj\Generated\", ""))
                .Should().BeEquivalentTo("root", "Microsoft",
                "John", "Marry", "Apple", "Alice", "Bob");
            foreach (var generatedFile in generatedFiles)
            {
                _fs.File.ReadAllText(generatedFile)
                    .Should()
                    .Be(generatedFile.Replace(@"c:\proj\Generated\", ""));
            }
        }
    }
}
