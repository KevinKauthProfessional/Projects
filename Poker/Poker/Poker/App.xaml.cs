using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Poker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected void OnStartup(object sender, StartupEventArgs args)
        {
            UriTypeConverter toUri = new UriTypeConverter();
            this.StartupUri = (Uri)toUri.ConvertFrom("MainWindow.xaml");
        }
    }
}
