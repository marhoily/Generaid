using System;
using System.Diagnostics;
using static System.StringComparison;

namespace Generaid
{
    [DebuggerDisplay("{FullName}")]
    internal sealed class CmpNode
    {
        private static readonly StringComparer 
            IgnoreCase = StringComparer.OrdinalIgnoreCase;

        public string DependentUpon { get; }
        public string FullName { get; }

        public CmpNode(string fullName, string dependentUpon = null)
        {
            FullName = fullName;
            DependentUpon = dependentUpon ?? "";
        }

        private bool Equals(CmpNode other)
        {
            return string.Equals(DependentUpon, other.DependentUpon, OrdinalIgnoreCase) 
                && string.Equals(FullName, other.FullName, OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as CmpNode;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (
                    IgnoreCase.GetHashCode(DependentUpon)*397) ^ 
                    IgnoreCase.GetHashCode(FullName);
            }
        }

        public static bool operator ==(CmpNode left, CmpNode right) => Equals(left, right);
        public static bool operator !=(CmpNode left, CmpNode right) => !Equals(left, right);
    }
}