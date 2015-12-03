using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CastReportingOfficePPTAddIn
{
    public partial class OverlayForm : Form
    {
        public OverlayForm()
        {
            InitializeComponent();
        }
        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            string text = e.Data.GetData(DataFormats.Text).ToString();
            this.Hide();
            Globals.ThisAddIn.OnDropOccurred(e.X, e.Y, e.Data.GetData(DataFormats.Text).ToString());
        }

        private void textBox2_DragOver(object sender, DragEventArgs e)
        {
            //Microsoft.Office.Interop.Word.Range range = (Microsoft.Office.Interop.Word.Range)Globals.ThisAddIn.Application.ActiveWindow.RangeFromPoint(e.X, e.Y);
            //range.Select();
        }
    }
}
