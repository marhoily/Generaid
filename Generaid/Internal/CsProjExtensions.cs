using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Generaid
{
    internal static class CsProjExtensions
    {
        private static readonly XmlNamespaceManager M;
        private static readonly XNamespace Ns = 
            "http://schemas.microsoft.com/developer/msbuild/2003";

        static CsProjExtensions()
        {
            M = new XmlNamespaceManager(new NameTable());
            M.AddNamespace("ns", Ns.NamespaceName);
        }

        private static string GetDependentUpon(this XContainer doc) 
            => doc.Element(Ns + "DependentUpon")?.Value;

        private static IEnumerable<XElement> FindByDirectory(this XContainer doc, string dir)
            => doc.XPathSelectElements("//ns:ItemGroup/ns:Compile", M)
                .Where(x => x.Attribute("Include")?.Value.StartsWith(dir) == true);

        private static void Insert(this XContainer doc, string preferredFolder, params CmpNode[] genNodes)
        {
            doc.XPathSelectElements("//ns:ItemGroup[ns:Compile]", M)
                .OrderByDescending(x => x
                    .XPathSelectElements("ns:Compile[@Include]", M)
                    .Count(y => y
                        .Attribute("Include")?.Value
                        .StartsWith(preferredFolder)==true))
                .FirstOrDefault()?
                .Add(genNodes.Select(n =>
                    new XElement(Ns + "Compile",
                        new XAttribute("Include", n.FullName),
                        new XElement(Ns + "DependentUpon", n.DependentUpon))));
        }

        private static XElement Find(this XContainer doc, CmpNode node) =>
            doc.XPathSelectElements($"//ns:ItemGroup/ns:Compile[@Include='{node.FullName}']", M)
            .SingleOrDefault(x => (x.GetDependentUpon() ?? "") == node.DependentUpon);

        private static CmpNode ToCmpNode(this XElement xElement) =>
            new CmpNode(xElement.Attribute("Include")?.Value, xElement.GetDependentUpon());

        public static bool Update(this XContainer proj,
            string projectDir, HashSet<CmpNode> newNodes)
        {
            var oldNodes = new HashSet<CmpNode>(
                proj.FindByDirectory(projectDir).Select(x => x.ToCmpNode()));

            var toAdd = newNodes.Except(oldNodes).ToList();
            var toRemove = oldNodes.Except(newNodes).ToList();
            if (toAdd.Count == 0 && toRemove.Count == 0)
                return false;
            foreach (var cmpNode in toRemove)
                proj.Find(cmpNode).Remove();
            foreach (var cmpNode in toAdd)
                proj.Insert(projectDir, cmpNode);
            return true;
        }
    }
}