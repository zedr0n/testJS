﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Awesomium.Core;
using Awesomium.Core.Data;
using Awesomium.Windows.Controls;

using System.Reflection;

namespace Backend
{
    public class JSHandlerBase
    {
        JSObject jsObject = null;
        public static string name = "jsObject";

        public List<JavascriptMethodHandler> handlers = new List<JavascriptMethodHandler>();

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

        public void add(JavascriptMethodHandler handler)
        {
            if (jsObject == null)
                return;
        }

        public void bind(WebControl webControl)
        {
            if(jsObject == null)
                jsObject = webControl.CreateGlobalJavascriptObject(name);
            foreach (MethodInfo method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (getDelegate<JavascriptMethodHandler>(method) != null)
                    handlers.Add(getDelegate<JavascriptMethodHandler>(method));
            }

            foreach(JavascriptMethodHandler handler in handlers)
            {
                jsObject.Bind(handler);
            }
        }

        public JSHandlerBase() { }
    }
    public class JSHandler : JSHandlerBase
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
            return "Submitted: " + textInput;
        }
        public void onTest()
        {
            // do nothing
        }
    }

}
