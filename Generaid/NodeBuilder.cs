using System;
using System.Collections;
using System.Collections.Generic;

namespace Generaid
{
    /// <summary>A node that represents file type</summary>
    /// <typeparam name="TGenerator">mode type</typeparam>
    public sealed class NodeBuilder<TGenerator> : IEnumerable
        where TGenerator : ITransformer
    {
        private readonly object _model;
        private readonly List<GenNode.Proto> _nodes = new List<GenNode.Proto>();

        /// <summary>Create a node without specifying a model, such
        ///     that the model will be resolved automatically </summary>
        public NodeBuilder() { }
        /// <summary>Create a node providing a model explicitly</summary>
        public NodeBuilder(object model) { _model = model; }
        /// <summary>Adds a child node</summary>
        public void Add<TNode>(NodeBuilder<TNode> item) 
            where TNode : ITransformer 
            => _nodes.Add(item.Build());
        internal GenNode.Proto Build() => new GenNode.Proto { Tp = typeof(TGenerator), Nodes = _nodes, Model = _model};
        IEnumerator IEnumerable.GetEnumerator() { throw new Exception(); }
    }
}