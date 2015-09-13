using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

using Awesomium.Core;
using NUnit.Framework;
using Backend;
using ClassDelegates;

// An STA thread will be created and used to run
// all the tests in the assembly
[assembly: RequiresSTA]

namespace NTest
{
    [TestFixture, RequiresSTA]
    public class GuiTest
    {
        Backend.MainWindow window = null;

        public void doTest(FrameEventHandler handler)
        {
            List<Exception> exceptions = new List<Exception>();
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                // Create and show the Window
                window = new Backend.MainWindow();

                window.Closed += (s, e) =>
                    Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);

                // Close the window after initialisation finished
                window.webControl.LoadingFrameComplete += (s, e) =>
                {
                    try
                    {
                        handler.Invoke(s, e);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                        window.Close();
                    }
                    window.Close();
                };
  
                window.Show();
                // Start the Dispatcher Processing
                Dispatcher.Run();
            }));

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
                    window.webControl.ExecuteJavascript("JS.App.doClick();");
                    Assert.AreEqual("Submitted: Test", (string)window.webControl.ExecuteJavascriptWithResult("JS.App.getOutput()"));
                });
        }
    }
}
