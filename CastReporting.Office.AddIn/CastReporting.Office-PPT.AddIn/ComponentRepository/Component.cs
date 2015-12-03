using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace ComponentRepository
{
    public class Component
    {
        #region Contructors

        public Component() { }

        public static Component CreateComponent(string name, string description, string options, ComponentType type, PowerPoint.Shape shape)
        {
            return new Component()
            {
                DisplayName = name,
                Description = description,
                Options = options,
                Shape = shape,
                CompoType = type,
            };
        }

        #endregion

        #region Properties

        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Options { get; set; }
        public ComponentType CompoType { get; set; }
        public PowerPoint.Shape Shape { get; set; }

        #endregion

        public void Dispose()
        {
            Marshal.ReleaseComObject(Shape);
        }
    }
}
