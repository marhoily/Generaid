using System;

namespace Generaid
{
    internal static class TypeExtensions
    {
        public static OneArgCtor Ctor(this Type tp) => OneArgCtor.From(tp);
    }
}