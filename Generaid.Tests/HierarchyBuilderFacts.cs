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
        private readonly MockFileSystem _fs = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    ["c:/proj/sample.proj"] = new MockFileData(
                        Utils.EmbeddedResource("Generaid.sample.proj"))
                });
        private readonly HierarchyBuilder _hierarchyBuilder;

        public HierarchyBuilderFacts()
        {
            _hierarchyBuilder = new HierarchyBuilder(
                _fs, "c:/proj/sample.proj", "Generated")
            {
                new NodeBuilder<ModelGenerator>(new Model())
                {
                    new NodeBuilder<CompanyGenerator>
                    {
                        new NodeBuilder<EmployeeGenerator>()
                    }
                }
            }
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

            _fs.AllFiles
                .Except(new[] { @"c:\proj\sample.proj" })
                .Should().BeEquivalentTo(
                    @"c:\proj\Generated\root",
                    @"c:\proj\Generated\Microsoft",
                    @"c:\proj\Generated\John",
                    @"c:\proj\Generated\Marry",
                    @"c:\proj\Generated\Apple",
                    @"c:\proj\Generated\Alice",
                    @"c:\proj\Generated\Bob");
        }

        [Fact]
        public void Generate_Write_Transformed_Text()
        {
            _hierarchyBuilder.Generate();

            var generatedFiles = _fs.AllFiles
                .Except(new[] { @"c:\proj\sample.proj" });

            foreach (var generatedFile in generatedFiles)
            {
                var expectedContent = generatedFile.Replace(@"c:\proj\Generated\", "");
                _fs.File
                    .ReadAllText(generatedFile)
                    .Should().Be(expectedContent);
            }
        }

        [Fact]
        public void Generate_Should_Delete_Old_Files_InThe_Folder()
        {
            _fs.File.WriteAllText(
                @"c:\proj\Generated\garbadge.txt", "blah");
            _hierarchyBuilder.Generate();
            _fs.AllFiles
                .Except(new[] { @"c:\proj\sample.proj" })
                .Should().BeEquivalentTo(
                    @"c:\proj\Generated\root",
                    @"c:\proj\Generated\Microsoft",
                    @"c:\proj\Generated\John",
                    @"c:\proj\Generated\Marry",
                    @"c:\proj\Generated\Apple",
                    @"c:\proj\Generated\Alice",
                    @"c:\proj\Generated\Bob");
        }
    }
}
