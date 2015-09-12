using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

using NUnit.Framework;
using Backend;

// An STA thread will be created and used to run
// all the tests in the assembly
[assembly: RequiresSTA]

namespace NTest
{
    [TestFixture, RequiresSTA]
    public class GuiTest
    {
        [Test]
        public void WindowTest()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                // Create and show the Window
                Backend.MainWindow tempWindow = new Backend.MainWindow();

                tempWindow.Closed += (s,e) =>
                    Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);                    

                // Close the window after initialisation finished
                tempWindow.webControl.LoadingFrameComplete += (s, e) =>
                    {
                        // simulate click
                        tempWindow.webControl.ExecuteJavascript("JS.App.doClick();");

                        // finish testing
                        tempWindow.Close();
                    };

                tempWindow.Show();
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
        }
    }
}
