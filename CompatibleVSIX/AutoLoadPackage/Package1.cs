using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE80;
using EnvDTE;

namespace CompatibleVSIX
{

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(Package1.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]

    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public sealed class Package1 : Package
    {
        public const string PackageGuidString = "bcb9777a-34eb-428c-a4cb-37b324e3cabd";

        #region Fields

        static DTE2 Dte2;
        static SolutionEventsListener SolutionEventsListener;
        static ServiceProvider ServiceProvider;        
        static System.Timers.Timer Timer;        
        static EnvDTE80.Events2 DteEvents;
        static ProjectItemsEvents DteProjectsEvents;
        const int TimerToFindDte = 5000;// 5 sec
        const int TimerToReviewRootDirectory = 1800000; //30mins
        static bool IsDteFounded = false;
        static DteInitializer dteInitializer;
        static IVsShell ShellService;

        #endregion

        public Package1()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            Timer = new System.Timers.Timer();
            InitializeDTE();
            InitializeSlnEvents();
            RunTimerService();
        }

        void InitializeDTE()
        {
            Dte2 = this.GetService(typeof(SDTE)) as DTE2;

            if (Dte2 == null && ShellService == null)
            {
                ShellService = this.GetService(typeof(SVsShell)) as IVsShell;
                dteInitializer = new DteInitializer(ShellService, this.InitializeDTE);
            }
        }

        void InitializeSlnEvents()
        {
            ServiceProvider = null;
            SolutionEventsListener = null;

            ServiceProvider = new ServiceProvider(Dte2 as IServiceProvider);
            SolutionEventsListener = new SolutionEventsListener(ServiceProvider);
            SolutionEventsListener.AfterSolutionLoaded += SolutionEventsListener_AfterSolutionLoaded;
        }

        void RunTimerService()
        {

            Timer.Enabled = false;
            Timer.Interval = TimerToFindDte;
            Timer.Enabled = true;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(Dte2?.Solution?.FullName))
            {
                IsDteFounded = false;
                InitializeDTE();
                return;
            }
            else
                IsDteFounded = true;


            if (IsDteFounded == false) return;

            try
            {
                Extensions.LogToOutputWindow(Dte2, $"Package has been sited by DTE @ {e.SignalTime}");
             
            }
            catch (System.Exception exp)
            {
                Extensions.LogToOutputWindow(Dte2, $"Error: {exp.Message}");
            }

            try
            {
                // Project events handlers
                if (DteProjectsEvents == null)
                {

                    DteEvents = Dte2.Events as EnvDTE80.Events2;
                    DteProjectsEvents = DteEvents.ProjectItemsEvents;
                    DteProjectsEvents.ItemAdded += PrjItemEvents_ItemAdded;
                    Extensions.LogToOutputWindow(Dte2, $"");
                }

            }
            catch (System.Exception exp)
            {
                Extensions.LogToOutputWindow(Dte2, $"Exception on project event handler: {exp.Message}");
            }

            if (Timer.Interval == TimerToFindDte && Dte2 != null)
            {
                Timer.Interval = TimerToReviewRootDirectory;
                Timer.Enabled = false;
                Timer.Enabled = true;
            }
        }

        void SolutionEventsListener_AfterSolutionLoaded()
        {           
            // TODO: if you want to do any action periodically add it to Timer event 
            RunTimerService();
        }

        void PrjItemEvents_ItemAdded(ProjectItem prjItem)
        {
            if (prjItem == null) return;
            if (prjItem.Name.EndsWith(".cs") == false) return;

            var itemPath = prjItem.Properties.Item("FullPath")?.Value.ToString();
            if (itemPath == null) return;
            if (System.IO.File.Exists(itemPath) == false) return;
            
            //TODO: if you want to manage your .cs file add your logic here
        }

    }
}
