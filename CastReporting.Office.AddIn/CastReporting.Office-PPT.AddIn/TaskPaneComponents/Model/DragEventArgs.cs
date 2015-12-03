using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentRepository;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace TaskPaneComponents
{
    public class DragEventArgs: EventArgs
    {
        public DragEventArgs(double x, double y, Component component)
        {
            X = x;
            Y = y;
            Component = component;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
        public Component Component { get; private set; }
    }
}
