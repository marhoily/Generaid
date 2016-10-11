using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace Generaid
{
    internal sealed class GenNode : INodeOwner
    {
        private ITransformer Transformer { get; }
        private GenNode NodeOwner => Owner as GenNode;
        private GenNode[] GenNodes { get; }
        private INodeOwner Owner { get; set; }
        private string Name => Transformer.Name;
        private readonly IFileSystem _fs;

        int INodeOwner.Level => Owner.Level + 1;
        public string GeneratedDirName => Owner.GeneratedDirName;
        public string DependentUpon
        {
            get
            {
                if (NodeOwner == null) return null;
                var ownerDir = _fs.Path.GetDirectoryName(NodeOwner.FullName);
                var myDir = _fs.Path.GetDirectoryName(FullName);
                return ownerDir != myDir ? null : NodeOwner.Name;
            }
        }
        public string FullName => GeneratedDirName + "\\" + Name;

        public GenNode(ITransformer transformer, IEnumerable<GenNode> nodes, IFileSystem fs)
        {
            if (transformer == null)
                throw new ArgumentNullException(nameof(transformer));
            Transformer = transformer;
            _fs = fs;
            GenNodes = nodes.ToArray();
        }

        public void SetOwner(INodeOwner owner)
        {
            Owner = owner;
            foreach (var node in GenNodes)
                node.SetOwner(this);
        }

        public sealed class Proto
        {
            public Type Tp;
            public List<Proto> Nodes;
            public object Model;
        }

        public IEnumerable<GenNode> GetDescendantsAndSelf()
        {
            yield return this;
            foreach (var child in GenNodes)
                foreach (var d in child.GetDescendantsAndSelf())
                    yield return d;
        }
        public bool DoGenerate => 
            (Transformer as ICanChooseToEscapeGeneration)?.DoNotGenerate != true;

        public void Generate(string projectRoot)
        {
            var file = _fs.Path.Combine(projectRoot, FullName);
            projectRoot.EnsureDirectoryExists(_fs, 
                _fs.Path.GetDirectoryName(file));
            _fs.File.WriteAllText(file, Transformer.TransformText());
        }
    }
}