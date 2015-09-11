using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Awesomium.Core;

namespace JSHandlers
{
    public class JSMethodHandler
    {
        public string name = "";
        public JavascriptMethodHandler aweHandler
        {
            get { return _aweHandler; }
        }
        public JSValue _aweHandler(object sender, JavascriptMethodEventArgs args)
        {
            if (handler == null)
                return null;

            // convert input arguments from JSValue to delegate argument parameters
            object[] _args = new object[args.Arguments.Length];
            for (int i = 0; i < args.Arguments.Length; ++i)
            {
                ParameterInfo paramInfo = handler.Method.GetParameters()[i];
                _args[i] = Convert.ChangeType(args.Arguments[i], paramInfo.ParameterType);
            }

            // convert return value to JSValue
            Type retType = handler.Method.ReturnType;
            object[] ret = new object[1];
            ret[0] = handler.DynamicInvoke(_args);
            foreach (ConstructorInfo cInfo in typeof(JSValue).GetConstructors())
            {
                if (cInfo.GetParameters().Length == 1 && cInfo.GetParameters()[0].ParameterType == retType)
                    return (JSValue)cInfo.Invoke(ret);
            }

            return null;
        }

        Delegate handler = null;

        public MethodInfo methodInfo
        {
            get { return handler.Method; }
        }

        public JSMethodHandler() { }
        public JSMethodHandler(Delegate handler)
        {
            this.name = handler.Method.Name;
            this.handler = handler;
        }
        public JSMethodHandler(string name, Delegate handler)
        {
            this.handler = handler;
            this.name = name;
        }

    }
}
