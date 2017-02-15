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
                    [@"c:\proj\sample.proj"] = new MockFileData(
                        Utils.EmbeddedResource("Generaid.sample.proj"))
                });
        private readonly HierarchyBuilder _hierarchyBuilder;
        private readonly Model _model = new Model();

        public HierarchyBuilderFacts()
        {
            _hierarchyBuilder = CreateHierarchyBuilder("Generated");
        }

        private HierarchyBuilder CreateHierarchyBuilder(string projectDir)
        {
            return new HierarchyBuilder(
                    _fs, @"c:\proj\sample.proj", projectDir)
                {
                    new NodeBuilder<ModelGenerator>(_model)
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
                ReadAllText(@"c:\proj\sample.proj"));
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
            Approvals.Verify(_fs.File.
                ReadAllText(@"c:\proj\sample.proj"));
        }
        [Fact]
        public void Generate_Should_Delete_Old_Files_InThe_Nested_Folder()
        {
            var hierarchyBuilder = CreateHierarchyBuilder(@"Gene\rated");
            _fs.File.WriteAllText(
                @"c:\proj\Gene\rated\garbadge.txt", "blah");
            hierarchyBuilder.Generate();
            _fs.AllFiles
                .Except(new[] { @"c:\proj\sample.proj" })
                .Should().BeEquivalentTo(
                    @"c:\proj\Gene\rated\root",
                    @"c:\proj\Gene\rated\Microsoft",
                    @"c:\proj\Gene\rated\John",
                    @"c:\proj\Gene\rated\Marry",
                    @"c:\proj\Gene\rated\Apple",
                    @"c:\proj\Gene\rated\Alice",
                    @"c:\proj\Gene\rated\Bob");
            Approvals.Verify(_fs.File.
                ReadAllText(@"c:\proj\sample.proj"));
        }
        [Fact]
        public void Generate_Should_Respect_Subfolders()
        {
            foreach (var company in _model.Companies)
                company.NeedSubfolder = true;

            _hierarchyBuilder.Generate();
            _fs.AllFiles
                .Except(new[] { @"c:\proj\sample.proj" })
                .Should().BeEquivalentTo(
                    @"c:\proj\Generated\companies\Microsoft",
                    @"c:\proj\Generated\companies\Apple",
                    @"c:\proj\Generated\root",
                    @"c:\proj\Generated\John",
                    @"c:\proj\Generated\Marry",
                    @"c:\proj\Generated\Alice",
                    @"c:\proj\Generated\Bob");
            Approvals.Verify(_fs.File.
                ReadAllText(@"c:\proj\sample.proj"));
        }
        [Fact]
        public void When_Depends_Updon_File_In_Nested_Folder_Should_Not_Reference_It_By_FullName()
        {
            foreach (var company in _model.Companies)
            {
                company.NeedSubfolder = true;
                foreach (var employee in company.Employees)
                    employee.NeedSubfolder = true;
            }

            _hierarchyBuilder.Generate();
          
            Approvals.Verify(_fs.File.
                ReadAllText(@"c:\proj\sample.proj"));
        }
        [Fact]
        public void DoNotGenerate_Should_Work()
        {
            _model.Companies[0].DoNotGenerate = true;
            foreach (var company in _model.Companies)
                company.NeedSubfolder = true;

            _hierarchyBuilder.Generate();
            _fs.AllFiles
                .Except(new[] { @"c:\proj\sample.proj" })
                .Should().BeEquivalentTo(
                    @"c:\proj\Generated\companies\Apple",
                    @"c:\proj\Generated\root",
                    @"c:\proj\Generated\John",
                    @"c:\proj\Generated\Marry",
                    @"c:\proj\Generated\Alice",
                    @"c:\proj\Generated\Bob");

            Approvals.Verify(_fs.File.
                ReadAllText(@"c:\proj\sample.proj"));
        }
    }
}
