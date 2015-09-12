using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Awesomium.Core;

using ClassDelegates;

namespace JSHandlers
{
    public class JSHandler
    {
        JSObject jsObject = null;
        public static string name = "jsObject";

        public List<JSMethodHandler> handlers = new List<JSMethodHandler>();

        public void addHandler(JSMethodHandler handler)
        {
            handlers.Add(handler);
        }
        public void addAllHandlers()
        {
            List<Delegate> allDelagates = this.getAllDelegates();
            foreach(Delegate dlg in allDelagates )
                addHandler(new JSMethodHandler(dlg));        
        }

        public void bind(JSMethodHandler handler)
        {
            if (jsObject == null)
                return;

            jsObject.Bind(handler.name, handler.aweHandler);
        }
        public void bind(JSObject jsObject)
        {
            if (jsObject == null)
                return;
            else
                this.jsObject = jsObject;

            foreach (MethodInfo method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (this.getDelegate<JavascriptMethodHandler>(method) != null)
                    jsObject.Bind(this.getDelegate<JavascriptMethodHandler>(method));
            }

            foreach (JSMethodHandler handler in handlers)
            {
                bind(handler);
            }


        }

        public JSHandler()
        {
            addAllHandlers();
        }
    }
}
