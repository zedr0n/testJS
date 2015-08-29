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

using Awesomium.Core;
using Awesomium.Core.Data;

namespace Backend
{
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

            jsObject.Bind("onClick",  JSHandler);
            
            //webControl.ExecuteJavascript("myMethod()");
            //webControl.ExecuteJavascript("myMethodExpectingReturn()");
            //var result = webView.ExecuteJavascriptWithResult("myMethodProvidingReturn('foo')");
            //Console.WriteLine(result.ToString());
        }

        private JSValue JSHandler(object sender, JavascriptMethodEventArgs args)
        {
            string txt = args.Arguments[0];
            return txt + " has been processed";
        }
    }
}
