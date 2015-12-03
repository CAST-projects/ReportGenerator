using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using TaskPaneComponents;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace CastReportingOfficePPTAddIn
{
    partial class TaskPane
    {
        Size minSize = new Size(300, 250);

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public ComponentsListViewModel ComponentslistViewModel;

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private UserControl1 ComponentslistView1;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            ComponentslistViewModel.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.ComponentslistView1 = new UserControl1();

            ComponentslistViewModel = new ComponentsListViewModel();
            ComponentslistViewModel.ReportDragEvent += ComponentslistViewModel_ReportDragEvent;
            ComponentslistView1.DataContext = ComponentslistViewModel;

            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.ComponentslistView1;
            //this.elementHost1.mi
            // 
            // TaskPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.AutoSize = true;
            //this.MinimumSize = minSize;
            this.Controls.Add(this.elementHost1);
            this.Resize += TaskPane_Resize;
            this.SizeChanged += TaskPane_SizeChanged;
            this.Name = "TaskPane";


            this.ResumeLayout(false);
        }

        #endregion
    }
}
