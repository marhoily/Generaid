T4 code generation aid library for VS
======

This library is a tool supporting a technique of code generation in VS that producess small maintainable *.tt and *.cs files.

## Code sample

```CSharp
var proj = Path.GetFullPath(Path.Combine(
    "../../../Sample.Tests/Sample.Tests.csproj"));

new HierarchyBuilder(proj, "Generated") {
    new NodeBuilder<Raw>(builder.Model) {
        new NodeBuilder<ChangeSet> {
            new NodeBuilder<Change>()
        },
        new NodeBuilder<TableSet> {
            new NodeBuilder<Table>()
        },
        new NodeBuilder<Columns>(),
        new NodeBuilder<PrimaryKey>()
    }
}
.With((IModel m) => m.GetEntityTypes())
.Generate();
```

Will help you to generate code like this: