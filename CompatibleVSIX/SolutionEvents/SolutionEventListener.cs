using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace CompatibleVSIX
{
    public class SolutionEventsListener : IVsSolutionEvents
    {
        private IVsSolution Solution;
        private uint SolutionEventsCookie;

        public event Action AfterSolutionLoaded;
        public event Action BeforeSolutionClosed;

        public SolutionEventsListener(IServiceProvider serviceProvider)
        {
            InitNullEvents();

            Solution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
            Solution?.AdviseSolutionEvents(this, out SolutionEventsCookie);
        }

        private void InitNullEvents()
        {
            AfterSolutionLoaded += () => { };
            BeforeSolutionClosed += () => { };
        }

        #region IVsSolutionEvents Members

        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            AfterSolutionLoaded();
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved)
        {
            BeforeSolutionClosed();
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Solution != null && SolutionEventsCookie != 0)
            {
                GC.SuppressFinalize(this);
                Solution.UnadviseSolutionEvents(SolutionEventsCookie);
                AfterSolutionLoaded = null;
                BeforeSolutionClosed = null;
                SolutionEventsCookie = 0;
                Solution = null;
            }
        }

        #endregion
    }
}
