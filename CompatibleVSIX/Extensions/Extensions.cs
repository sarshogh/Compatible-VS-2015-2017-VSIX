using System.Linq;
using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace CompatibleVSIX
{
    public static class Extensions
    {        
        public static void LogToOutputWindow(DTE2 dte, string message)
        {
            if (dte == null) return;
            if (string.IsNullOrEmpty(message)) return;

            try
            {
                var window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
                var outputWindow = (OutputWindow)window.Object;
                outputWindow.ActivePane.Activate();
                outputWindow.ActivePane.OutputString(message + Environment.NewLine);
            }
            catch { }
        }
    }
}
