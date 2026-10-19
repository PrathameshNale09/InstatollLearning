using InstaTollLearningApp.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace InstaTollLearningApp
{
    internal static class Program
    {
        private static readonly Logger Logger =
    LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ActivitySummaryService.Start();

            Application.ApplicationExit += (sender, args) =>
            {
                ActivitySummaryService.Stop();
            };
            //Logger.Info("InstaToll application started.");
            Application.Run(new LoginForms());
        }
    }
}
