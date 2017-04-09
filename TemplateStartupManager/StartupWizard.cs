using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TemplateStartupManager
{
    public class StartupWizard : IWizard, IDisposable
    {
        static DTE2 CurrnetDte;
        static string CurrentSolutionName;
        static string CurrentSolutionPath;
        static Dictionary<string, string> ReplacementsDictionary = new Dictionary<string, string>();
        bool Disposed;

        #region Iwizard methods

        public void BeforeOpeningFile(global::EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(global::EnvDTE.Project project)
        {
        }

        public void ProjectItemFinishedGenerating(global::EnvDTE.ProjectItem projectItem)
        {
        }

        public bool ShouldAddProjectItem(string filePath) => true;

        #endregion

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            CurrnetDte = (DTE2)automationObject;
            ReplacementsDictionary = replacementsDictionary;
            CurrentSolutionPath = replacementsDictionary["$destinationdirectory$"];
        }

        /// <summary>
        /// after template generation, this method is being invoked by Iwizard
        /// </summary>
        public void RunFinished()
        {
            try
            {
                GetSolutionName();
                              
            }
            catch (Exception exp)
            {
                LogTo.File(exp.Message);
                LogTo.OutputWindow(CurrnetDte, exp.Message);
            }
            finally
            {
                CurrnetDte = null;
                ReplacementsDictionary = null;
            }
        }

        private void OpenSolution()
        {
            if (CurrnetDte.Version.Contains("14"))
                CurrnetDte.Solution.Open(GetSlnFileFullPath());
        }

        private void CloseMemorySolution()
        {
            CurrnetDte.Solution.Close(SaveFirst: false);
        }
       
        void GetSolutionName()
        {
            do
            {
                try
                {
                    CurrentSolutionName = ((SolutionClass)CurrnetDte?.Solution)?.Properties.Item("Name").Value?.ToString();
                }
                catch
                {
                    return;
                }
            } while (CurrentSolutionName == string.Empty);
        }

        Project FindProjectWithName(string name)
        {
            var projects =  CurrnetDte.Solution?.Projects;

            if (projects == null) return null;

            foreach (Project proj in projects)
            {
                if (proj.Name == name) return proj;
                
                foreach (ProjectItem item in proj.ProjectItems)
                {
                    var subProj = item.SubProject as Project;

                    if (subProj == null) continue;
                    if (subProj.Name != name) continue;

                    return subProj;
                }                
            }

            return null;
        }
        
        string GetSlnFileFullPath()
        {
            return $@"{ Directory.GetParent(CurrentSolutionPath)}\{CurrentSolutionName}.sln";
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    CurrnetDte = null;
                    CurrnetDte.Quit();
                }
            }
            //dispose unmanaged resources
            Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
    }
}
