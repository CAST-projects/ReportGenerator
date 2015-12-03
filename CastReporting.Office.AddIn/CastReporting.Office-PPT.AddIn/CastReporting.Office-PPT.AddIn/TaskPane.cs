using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using TaskPaneComponents;
using ComponentRepository;
using System.IO;
using System.Reflection;

namespace CastReportingOfficePPTAddIn
{
    public partial class TaskPane : UserControl
    {
        bool canLoadData = true;

        public TaskPane()
        {
            InitializeComponent();
        }

        public void LoadData()
        {
            if (canLoadData)
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                            Properties.Resources.ComponentsFile);

                ComponentsRepository componentsRepository = new ComponentsRepository(path);
                ComponentslistViewModel.CreateAllComponents(componentsRepository);

                canLoadData = false;
            }
        }

        void TaskPane_Resize(object sender, EventArgs e)
        {
            //if (this.Size.Width < minSize.Width)
            //{
            //    this.Size = new Size(minSize.Width, this.Size.Height);
            //    this.Refresh();
            //}
            //else if (this.Size.Height < minSize.Height)
            //{
            //    this.Size = new Size(this.Size.Width, minSize.Height);
            //    this.Refresh();
            //}
            this.elementHost1.Size = this.Size;
        }

        void TaskPane_SizeChanged(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.ctp != null)
            {
                if (Globals.ThisAddIn.ctp.DockPosition == Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionTop
                    && Globals.ThisAddIn.ctp.Height <= minSize.Height)
                {
                    SendKeys.Send("{ESC}");

                    Globals.ThisAddIn.ctp.Height = minSize.Height;
                }

                if (Globals.ThisAddIn.ctp.Width <= minSize.Width)
                {
                    SendKeys.Send("{ESC}");

                    Globals.ThisAddIn.ctp.Width = minSize.Width;
                }
            }
        }

        void ComponentslistViewModel_ReportDragEvent(object sender, TaskPaneComponents.DragEventArgs e)
        {
            try
            {
                PowerPoint.Presentation presentation = Globals.ThisAddIn.Application.ActivePresentation;

                if (presentation != null)
                {
                    PowerPoint.Application ppApp = Globals.ThisAddIn.Application;
                    PowerPoint.SlideRange ppSR = ppApp.ActiveWindow.Selection.SlideRange;

                    e.Component.Shape.Copy();
                    ppSR.Shapes.Paste();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Properties.Resources.DragDropErrMsg, ex.Message));
            }
        }
    }
}
