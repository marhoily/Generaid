using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Generaid
{
    using M = IEnumerable<Tuple<ITransformer, object>>;
    using C = IDictionary<Type, Func<object, IEnumerable<object>>>;

    internal sealed class GenHierarchy : INodeOwner
    {
        private readonly GenNode[] _genNodes;

        public string ProjectPath { get; }
        public string ProjectDir { get; }
        int INodeOwner.Level => 0;

        public GenHierarchy(string projectPath,
            string projectDir, List<GenNode.Proto> nodes, C converters)
        {
            ProjectPath = projectPath;
            ProjectDir = projectDir;
            _genNodes = nodes
                .SelectMany(n => Expand(n, converters, n.Model))
                .ToArray();
            foreach (var genNode in _genNodes)
                genNode.SetOwner(this);
        }

        private IEnumerable<GenNode> GetAllNodes() 
            => _genNodes.SelectMany(x => x.GetDescendantsAndSelf());

        private static IEnumerable<GenNode> Expand(GenNode.Proto node, C converters, object model)
        {
            return Choose(node.Tp.Ctor(), converters, node.Model ?? model)
                .Select(o => new GenNode(o.Item1, node.Nodes.SelectMany(
                    x => Expand(x, converters, o.Item2))));
        }

        private static M Choose(OneArgCtor ctor, C converters, object model)
        {
            if (ctor.NoArgs)
                return new[] { Tuple.Create(ctor.Invoke(null), model) };
            if (ctor.ArgType.IsInstanceOfType(model))
                return new[] { Tuple.Create(ctor.Invoke(model), model) };
            return converters[ctor.ArgType](model)
                .Select(m => Tuple.Create(ctor.Invoke(m), m));
        }

        public void Generate()
        {
            var doc = XDocument.Load(ProjectPath);
            var nodes = GetAllNodes().ToList();
            foreach (var node in nodes)
                node.Generate(Path.GetDirectoryName(ProjectPath));
            var set = new HashSet<CmpNode>(nodes.Select(
                n => new CmpNode(n.FullName, n.DependentUpon)));
            if (doc.Update(ProjectDir, set))
                doc.Save(ProjectPath);
        }
    }
}