using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace Generaid
{
    using C = Dictionary<Type, Func<object, IEnumerable<object>>>;

    /// <summary>Use to build a hierarchy</summary>
    public sealed class HierarchyBuilder : IEnumerable
    {
        private readonly IFileSystem _fs;
        private readonly string _projectPath;
        private readonly string _projectDir;
        private readonly List<GenNode.Proto> _nodes = new List<GenNode.Proto>();
        private readonly C _registrations = new C();

        /// <summary>Use to build a hierarchy</summary>
        public HierarchyBuilder(string projectPath, string projectDir)
            : this(new FileSystem(), projectPath, projectDir)
        {
            
        }
        /// <summary>Version for tests</summary>
        internal HierarchyBuilder(IFileSystem fs, string projectPath, string projectDir)
        {
            _fs = fs;
            _projectPath = projectPath;
            _projectDir = projectDir;
        }
        /// <summary>Adds nodes</summary>
        public void Add<T>(NodeBuilder<T> item) 
            where T : ITransformer 
            => _nodes.Add(item.Build());
        /// <summary>Tells builder how to resolve models</summary>
        public HierarchyBuilder With<TK, TV>(Func<TK, IEnumerable<TV>> func)
        {
            _registrations.Add(typeof(TV),
                x => (IEnumerable<object>)func((TK)x));
            return this;
        }
        /// <summary>Go and generate code, update csproj file, etc.</summary>
        public void Generate() => new GenHierarchy(
            _projectPath, _projectDir, _nodes, _registrations, _fs).Generate();
        IEnumerator IEnumerable.GetEnumerator() { throw new Exception(); }
    }
}