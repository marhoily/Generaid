using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

namespace Generaid
{
    [UseReporter(typeof(AraxisMergeReporter))]
    public sealed class ModelGenerator : ITransformer
    {
        [Fact]
        public void Test()
        {
            var fs = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    ["c:/proj/sample.proj"] = new MockFileData(EmbeddedResource("Generaid.sample.proj"))
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
        }
        private static string EmbeddedResource(string resource)
        {
            var asm = typeof(ModelGenerator).Assembly;
            var stream = asm.GetManifestResourceStream(resource);
            if (stream == null)
                throw new Exception("Following resources are available:\r\n" +
                    string.Join("\r\n", asm.GetManifestResourceNames().Select((s, i) => $"{i}) '{s}'")));
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
        public string Name => "model.cs";
        public string TransformText() => "blah";
    }
}
