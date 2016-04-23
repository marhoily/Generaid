using System.IO;
using ApprovalTests;
using Xunit;

namespace Generaid
{
    public sealed class ModelGenerator : ITransformer
    {
        [Fact]
        public void Test()
        {
            using (var folder = new TempFolder())
            {
                var proj = folder.CopyFileFrom("../../sample.proj");
                var h = new HierarchyBuilder(proj, "Generated") {
                    new NodeBuilder<ModelGenerator>(new Model()) {
                        new NodeBuilder<CompanyGenerator> {
                            new NodeBuilder<EmployeeGenerator>() } } }
                    .With((Model m) => m.Companies)
                    .With((Company c) => c.Employees);
                h.Generate();
                h.Generate();
                Approvals.Verify(File.ReadAllText(proj));
              //  Process.Start(folder.Pin());
            }
        }

        public string Name => "model.cs";
        public string TransformText() => "blah";
    }
}
