using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TNL.Structures
{
    using Entities;
    using Interfaces;
    using Utils;

    public class FunctorDecl<T> : IFunctor where T : EventConnection
    {
        public Delegate MethodDelegate;
        public object[] Parameters;
        public object[] Arguments;
        public Type[] ParamTypes;

        public FunctorDecl(string methodName, Type[] paramTypes)
        {
            var tType = typeof(T);

            var method = tType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            ParamTypes = paramTypes;

            var list = paramTypes.ToList();
            list.Insert(0, tType);

            MethodDelegate = Delegate.CreateDelegate(Expression.GetActionType(list.ToArray()), method);
        }

        public void Set(object[] parameters)
        {
            Parameters = parameters;
        }

        public void Read(BitStream stream)
        {
            Arguments = new object[ParamTypes.Length + 1];

            for (var i = 0; i < ParamTypes.Length; ++i)
                Arguments[1 + i] = ReflectedSerializer.Read(stream, ParamTypes[i]);
        }

        public void Write(BitStream stream)
        {
            if (Parameters == null)
                return;

            foreach (var t in Parameters)
                ReflectedSerializer.Write(stream, t, t.GetType());
        }

        public void Dispatch(object obj)
        {
            if (MethodDelegate == null || Arguments == null || obj == null || (obj as T) == null)
                return;

            try
            {
                Arguments[0] = obj;
                MethodDelegate.DynamicInvoke(Arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid type?? Expected: {0} | Found: {1}", typeof(T).Name, obj.GetType().Name);
                Console.WriteLine(e);
            }
        }
    }
}
