using EnvDTE;
using EnvDTE80;
using System;

namespace TemplateStartupManager
{
    public static class LogTo
    {
        static string FilePathToLog = "c:\\temp\\Template.txt";

        public static void OutputWindow(DTE2 dte, string message)
        {
            try
            {
                var window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
                var outputWindow = (OutputWindow)window.Object;
                outputWindow.ActivePane.Activate();
                outputWindow.ActivePane.OutputString(message + Environment.NewLine);
            }
            catch { }
        }

        public static void File(string msg)
        {
            System.IO.File.AppendAllText(FilePathToLog, msg + Environment.NewLine);
        }
    }
}
