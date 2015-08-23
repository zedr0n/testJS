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
            // Create a WebSession.
            WebSession session = WebCore.CreateWebSession(new WebPreferences()
            {
                SmoothScrolling = true,
            });
            session.AddDataSource("core", new ResourceDataSource(ResourceType.Embedded));

            InitializeComponent();

            webControl.WebSession = session;
        }
    }
}
