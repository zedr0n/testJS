using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Awesomium.Core;
using Awesomium.Core.Data;
using Awesomium.Windows.Controls;

using System.Reflection;

namespace Backend
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
            for(int i = 0; i < args.Arguments.Length; ++i)
            {
                ParameterInfo paramInfo = handler.Method.GetParameters()[i];
                _args[i] = Convert.ChangeType(args.Arguments[i],paramInfo.ParameterType);
            }

            // convert return value to JSValue
            Type retType = handler.Method.ReturnType;
            object[] ret = new object[1];
            ret[0] =  handler.DynamicInvoke(_args) ;
            foreach(ConstructorInfo cInfo in typeof(JSValue).GetConstructors())
            {
                if(cInfo.GetParameters().Length == 1 && cInfo.GetParameters()[0].ParameterType == retType)
                       return (JSValue) cInfo.Invoke(ret);
            }

            return null;
        }

        Delegate handler = null;

        public MethodInfo methodInfo
        {
            get { return handler.Method; }
        }

        public JSMethodHandler() {}
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

    public class JSHandlerBase
    {
        JSObject jsObject = null;
        public static string name = "jsObject";

        public List<JSMethodHandler> handlers = new List<JSMethodHandler>();

        public T getByName<T>(string methodName)
        {
            Delegate handler = null;
            try
            {
                handler = Delegate.CreateDelegate(typeof(T), this, methodName);
            }
            catch { }
            return (T)Convert.ChangeType(handler, typeof(T));
        }
        public T getDelegate<T>(MethodInfo methodInfo)
        {
            Delegate dlg = null;
            try
            {
                dlg = methodInfo.CreateDelegate(typeof(T), this);
            }
            catch (System.Exception ) { }
            return (T)Convert.ChangeType(dlg, typeof(T));
        }

        public void add(JSMethodHandler handler)
        {
            handlers.Add(handler);
        }

        public virtual void addMethods()
        {
            foreach (MethodInfo methodInfo in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
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
                Delegate d = methodInfo.CreateDelegate(delegateType, this);
                add(new JSMethodHandler(d));
            }
        }

        public void bind(JSMethodHandler handler)
        {
            if (jsObject == null)
                return;

            jsObject.Bind(handler.name,handler.aweHandler);
        }
        public void bind(WebControl webControl)
        {
            if(jsObject == null)
                jsObject = webControl.CreateGlobalJavascriptObject(name);
            foreach (MethodInfo method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (getDelegate<JavascriptMethodHandler>(method) != null)
                    jsObject.Bind(getDelegate<JavascriptMethodHandler>(method));
            }

            foreach(JSMethodHandler handler in handlers)
            {
                bind(handler);
            }

             
        }

        public JSHandlerBase() 
        {
            addMethods();
        }
    }
    public class JSHandler : JSHandlerBase
    {
        // proper handlers
        public string onClick(string textInput)
        {
            return "Submitted: " + textInput;
        }
        public void onTest()
        {
            // do nothing
        }

        public JSHandler()
        {
            add(new JSMethodHandler("onTest2", (Func<double, double,double>)((x,y) => x * y)));
        }
    }

}
