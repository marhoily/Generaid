T4 code generation aid library for VS
======

This library is a tool supporting a technique of code generation in VS that producess small maintainable *.tt and *.cs files.

## How to use:

 1. Create a console application project and install this library:

	PM> Install-Package Generaid

I usually call this project "Generator". Set it as a startup project so that every time you want to generate code in your solution it runs.

2. Add a class library project for the generated code. Let's call it "Generated".
3. Add a "Runtime Text Template" project item. Let's call it "MyTemplate.tt". Add the following code in it:
```T4
<#@ template language="C#" #>

public sealed class HelloWorld
{
}
```
4. Add a code file with a partial class class beside it, to make ourt template implement `ITransformation` interface, like this:

```CSharp
partial class MyTemplate : Generaid.ITransformer
{
    public string Name => "fileName.cs";
}
```

Make sure that the namespaces match between the halves of the partial class. The `TransformText()` method should be implement by the part generated from the *.tt file.

5. To your `Main()` add the following code:

```CSharp
var builder = new HierarchyBuilder("../../Generated/Generated.csproj", "Folder")
{
    new NodeBuilder<MyTemplate>()
};
builder.Generate();
```

6. Hit F5! If everything runs OK you