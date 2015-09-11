using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace JSHandlers
{
    public static class Methods
    {
        public static T getByName<T>(this object obj, string methodName)
        {
            Delegate handler = null;
            try
            {
                handler = Delegate.CreateDelegate(typeof(T), obj, methodName);
            }
            catch { }
            return (T)Convert.ChangeType(handler, typeof(T));
        }
        public static T getDelegate<T>(this object obj, MethodInfo methodInfo)
        {
            Delegate dlg = null;
            try
            {
                dlg = methodInfo.CreateDelegate(typeof(T), obj);
            }
            catch (System.Exception) { }
            return (T)Convert.ChangeType(dlg, typeof(T));
        }
        public static List<Delegate> getAllDelegates(this object obj)
        {
            List<Delegate> allDelegates = new List<Delegate>();
            foreach (MethodInfo methodInfo in obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                List<Type> args = new List<Type>(
                    methodInfo.GetParameters().Select(p => p.ParameterType));
                Type delegateType;
                if (methodInfo.ReturnType == typeof(void))
                {
                    delegateType = Expression.GetActionType(args.ToArray());
                }
                else
                {
                    args.Add(methodInfo.ReturnType);
                    delegateType = Expression.GetFuncType(args.ToArray());
                }
                allDelegates.Add(methodInfo.CreateDelegate(delegateType, obj));
            }
            return allDelegates;
        }
    }
}
