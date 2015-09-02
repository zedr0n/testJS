using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Awesomium.Core;
using Awesomium.Core.Data;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace Backend
{
    public class Param
    {
        public string paramType;
        public string paramName;

        public Param() { }
        public Param(string paramType, string paramName)
        {
            this.paramType = paramType;
            this.paramName = paramName;
        }
    }

    public class Method
    {
        public string name;
        public List<Param> parameters = new List<Param>();
        public string returnType;

        public Method() { }

        public Method(MethodInfo methodInfo)
        {
            name = methodInfo.Name;
            returnType = methodInfo.ReturnType.Name.ToLower();
            foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
            {
                parameters.Add(new Param(paramInfo.ParameterType.Name.ToLower(), paramInfo.Name));
            }
        }
    }

    public class JSHandler
    {
        // handler stubs for Awesomium
        public JSValue onClick(object sender, JavascriptMethodEventArgs args)
        {
            return (JSValue)onClick(args.Arguments[0]);
        }

        public JSValue onTest(object sender, JavascriptMethodEventArgs args)
        {
            onTest();
            return null;
        }

        // proper handlers
        public string onClick(string textInput)
        {
            return textInput + "has been processed";
        }

        public void onTest()
        {
            // do nothing
        }

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
            catch (System.Exception ex) { }
            return (T)Convert.ChangeType(dlg, typeof(T));

        }

        public List<Method> methods = new List<Method>();

        public JSHandler()
        {
            List<string> methodNames = new List<string>();
            foreach (MethodInfo methodInfo in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (getDelegate<JavascriptMethodHandler>(methodInfo) != null)
                    methodNames.Add(methodInfo.Name);
            }


            foreach (MethodInfo methodInfo in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (methodNames.Contains(methodInfo.Name) && getDelegate<JavascriptMethodHandler>(methodInfo) == null)
                {
                    Method method = new Method(methodInfo);
                    methods.Add(method);
                }
            }
        }
    }

}
