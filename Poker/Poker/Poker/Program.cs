

namespace Poker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    
    public class Program
    {
        private static MainWindow mainWindow = new MainWindow();

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                mainWindow.InitializeComponent();
                mainWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                FlattenExceptionAndDumpLog(ex);
            }
        }

        private static void FlattenExceptionAndDumpLog(Exception ex)
        {
            Exception flatEx = ex;
            if (ex is AggregateException)
            {
                AggregateException aggEx = ex as AggregateException;
                flatEx = aggEx.Flatten().InnerException;
            }

            mainWindow.LogBuilder.AppendLine(string.Format("{0}: {1}", DateTime.Now, flatEx));

            using (StreamWriter writer = new StreamWriter("ErrorDump.log"))
            {
                writer.Write(mainWindow.LogBuilder.ToString());
            }
        }
    }
}
