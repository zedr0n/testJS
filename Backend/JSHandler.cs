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

        public JSHandler() { }
    }

}
