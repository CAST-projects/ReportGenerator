using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;

namespace CastReportingOfficePPTAddIn
{
    public partial class ThisAddIn
    {
        #region Fields

        public CustomTaskPane ctp = null;
        public TaskPane tp = null;
        Ribbon1 ribbon;

        #endregion

        #region Private Methods

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Application.PresentationBeforeClose += Application_PresentationBeforeClose;

            tp = new TaskPane();
            ctp = this.CustomTaskPanes.Add(tp, Properties.Resources.TaskPaneTitle);
            ctp.Width = 300;

            ctp.VisibleChanged += new EventHandler(ctp_VisibleChanged);
        }

        void Application_PresentationBeforeClose(PowerPoint.Presentation Pres, ref bool Cancel)
        {
            tp.Dispose();
        }

        private void ctp_VisibleChanged(object sender, EventArgs e)
        {
            ribbon.Refresh();
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            ctp.VisibleChanged -= new EventHandler(ctp_VisibleChanged);
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            ribbon = new Ribbon1();
            return ribbon;
        }

        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
