using System;
using System.Windows;
using System.Diagnostics;
using System.Reflection;

using Awesomium.Core;
using Awesomium.Core.Data;

using JSHandlers;

namespace Backend
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private JSObject jsObject = null;
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
            session.AddDataSource("core", new ResourceDataSource(ResourceType.Embedded,Assembly.GetExecutingAssembly()));

            InitializeComponent();
            webControl.DocumentReady += onDocumentReady;
            webControl.ConsoleMessage += onConsoleMessage;
            webControl.LoadingFrameComplete += onLoadingFrameComplete;

            webControl.WebSession = session;
        }

        private static void onConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Debug.Print("{0} at {1}: {2} at '{3}'", e.EventName, e.LineNumber, e.Message, e.Source);
        }
        private void onDocumentReady(object sender, UrlEventArgs urlEventArgs)
        {
            webControl.DocumentReady -= onDocumentReady;

            jsObject = webControl.CreateGlobalJavascriptObject("jsObject");
            ButtonHandler jsHandler = new ButtonHandler();
            jsHandler.bind(jsObject);

        }

        void onLoadingFrameComplete(object sender, FrameEventArgs e)
        {
            webControl.LoadingFrameComplete -= onLoadingFrameComplete;

            webControl.ExecuteJavascriptWithResult("JS.App.setText();");
            //dynamic document = (Awesomium.Core.JSObject)Html5.ExecuteJavascriptWithResult("document");
        }
    }
}
