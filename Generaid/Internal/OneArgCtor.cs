using System;
using System.Linq;
using System.Reflection;

namespace Generaid
{
    internal sealed class OneArgCtor
    {
        public Type ArgType { get; }
        public bool NoArgs => ArgType == null;
        public Func<object, ITransformer> Invoke { get; }

        private OneArgCtor(ConstructorInfo ctor)
        {
            var arg = ctor.GetParameters().SingleOrDefault();
            if (arg == null)
            {
                Invoke = _ => (ITransformer)ctor.Invoke(new object[0]);
            }
            else
            {
                ArgType = arg.ParameterType;
                Invoke = a => (ITransformer)ctor.Invoke(new[] { a });
            }
        }

        public override string ToString() => $"{ArgType?.Name}";

        public static OneArgCtor From(Type tp)
        {
            var ctor = tp.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .SingleOrDefault(c => c.GetParameters().Length <= 1);
            return ctor != null ? new OneArgCtor(ctor) : null;
        }
    }
}