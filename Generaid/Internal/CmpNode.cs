using System;
using System.Diagnostics;

namespace Generaid
{
    [DebuggerDisplay("{FullName}")]
    internal sealed class CmpNode
    {
        private static readonly StringComparer 
            Case = StringComparer.OrdinalIgnoreCase;

        public string DependentUpon { get; }
        public string FullName { get; }

        public CmpNode(string fullName, string dependentUpon = null)
        {
            FullName = fullName;
            DependentUpon = dependentUpon ?? "";
        }

        private bool Equals(CmpNode other)
        {
            return string.Equals(DependentUpon, other.DependentUpon, StringComparison.OrdinalIgnoreCase) 
                && string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
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
                    Case.GetHashCode(DependentUpon)*397) ^ 
                    Case.GetHashCode(FullName);
            }
        }

        public static bool operator ==(CmpNode left, CmpNode right) => Equals(left, right);
        public static bool operator !=(CmpNode left, CmpNode right) => !Equals(left, right);
    }
}