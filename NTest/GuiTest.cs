using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using Awesomium.Core;
using NUnit.Framework;

// An STA thread will be created and used to run
// all the tests in the assembly
[assembly: RequiresSTA]

namespace NTest
{
    [TestFixture, RequiresSTA]
    public class GuiTest
    {
        Backend.MainWindow _window;

        private void doTest(FrameEventHandler handler)
        {
            List<Exception> exceptions = new List<Exception>();
            var newWindowThread = new Thread(() =>
            {
                // Create and show the Window
                _window = new Backend.MainWindow();

                _window.Closed += (s, e) =>
                    Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                // Close the window after initialisation finished
                _window.webControl.LoadingFrameComplete += (s, e) =>
                {
                    try
                    {
                        handler.Invoke(s, e);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                        _window.Close();
                    }
                    _window.Close();
                };
  
                _window.Show();
                // Start the Dispatcher Processing
                Dispatcher.Run();
            });

            // Set the apartment state
            newWindowThread.SetApartmentState(ApartmentState.STA);
            // Make the thread a background thread
            newWindowThread.IsBackground = true;
            // Start the thread
            newWindowThread.Start();
            // Wait for thread to finish before completing test
            newWindowThread.Join();

            foreach (Exception ex in exceptions)
                throw ex;
        }
        
        [Test]
        public void clickTest()
        {
            doTest( (s,e) =>
                {
                    // simulate click
                    _window.webControl.ExecuteJavascript("JS.App.doClick();");
                    Assert.AreEqual("http://bridge.net", (string)_window.webControl.ExecuteJavascriptWithResult("JS.App.getOutput()"));
                });
        }
    }
}
