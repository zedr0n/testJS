using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Reflection;

using Awesomium.Core;
using Awesomium.Core.Data;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

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

        public Method() {}

        public Method(MethodInfo methodInfo)
        {
            name = methodInfo.Name;
            returnType = methodInfo.ReturnType.ToString();
            foreach(ParameterInfo paramInfo in methodInfo.GetParameters())
            {
                parameters.Add(new Param(paramInfo.ParameterType.ToString(), paramInfo.Name));
            }
        }
    }
        
    public class JSHandler
    {
        // handler stubs for Awesomium
        public JSValue onClick(object sender, JavascriptMethodEventArgs args)
        {
            return (JSValue) onClick(args.Arguments[0]);
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
                dlg = methodInfo.CreateDelegate(typeof(T),this);
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

        // XML serialisation
        public string writeToXML()
        {
            XmlSerializer x = new XmlSerializer(GetType());
            StringWriter sw = new StringWriter();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            x.Serialize(sw, this, ns);
            return sw.ToString();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (!WebCore.IsInitialized)
                WebCore.Initialize(new WebConfig()
                {
                    HomeURL = new Uri("http://localhost"),
                    RemoteDebuggingPort = 8001,

                });
            
            // Create a WebSession.
            WebSession session = WebCore.CreateWebSession(new WebPreferences()
            {
                SmoothScrolling = true
            });
            session.AddDataSource("core", new ResourceDataSource(ResourceType.Embedded));


            InitializeComponent();
            webControl.DocumentReady += OnDocumentReady;
            webControl.ConsoleMessage += OnConsoleMessage;

            webControl.WebSession = session;
            
        }

        private void OnConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Debug.Print("{0} at {1}: {2} at '{3}'", e.EventName, e.LineNumber, e.Message, e.Source);
        }

        private void OnDocumentReady(object sender, UrlEventArgs urlEventArgs)
        {
            webControl.DocumentReady -= OnDocumentReady;
            JSObject jsObject = webControl.CreateGlobalJavascriptObject("jsObject");

            //jsObject.Bind(((JavascriptMethodHandler)onClick).Method.Name, onClick);
            //jsObject.Bind(this.GetMemberName(x => x.onClick(null, null)), onClick);
            //jsObject.Bind(onClick);

            JSHandler jsHandler = new JSHandler();

            foreach(MethodInfo method in jsHandler.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (jsHandler.getDelegate<JavascriptMethodHandler>(method) != null)
                    jsObject.Bind(jsHandler.getDelegate<JavascriptMethodHandler>(method));
            }

            string xml = jsHandler.writeToXML();

            //jsObject.Bind(getByName(methodName));
            //string name = StaticReflection.GetMemberName<MainWindow>( x => x.onClick(null,null));
            //string name2 = this.GetMemberName(x => x.onClick(null, null));
            
            //webControl.ExecuteJavascript("myMethod()");
            //webControl.ExecuteJavascript("myMethodExpectingReturn()");
            //var result = webView.ExecuteJavascriptWithResult("myMethodProvidingReturn('foo')");
            //Console.WriteLine(result.ToString());
        }



    }
}
