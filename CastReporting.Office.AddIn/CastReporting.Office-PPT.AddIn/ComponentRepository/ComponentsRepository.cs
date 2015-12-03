using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;

namespace ComponentRepository
{
    public class ComponentsRepository
    {
        #region Fields

        List<Component> components;

        #endregion

        #region Constructor

        public ComponentsRepository(string data)
        {
            components = loadComponents(data);
        }

        #endregion

        #region Properties

        public List<Component> Components
        {
            get { return components; }
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            foreach (Component comp in components)
                comp.Dispose();
        }

        #endregion

        #region Private Helpers

        private List<Component> loadComponents(string templateFile)
        {
            PowerPoint.Application app = new PowerPoint.Application();
            PowerPoint.Presentation pres = app.Presentations.Open(templateFile, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
            var slides = pres.Slides;

            List<Component> components = new List<Component>();

            foreach (PowerPoint.Slide slide in slides)
            {
                components.Add(getComponent(slide));
            }

            return components;
        }

        Component getComponent(PowerPoint.Slide slide)
        {
            string name = null, desc = null, options = null;
            PowerPoint.Shape shape = null;
            ComponentType type = default(ComponentType);

            //Iterating through all shapes inside the slide
            foreach (PowerPoint.Shape _shape in slide.Shapes)
            {
                if (!string.IsNullOrEmpty(_shape.AlternativeText))
                {
                    string alt = _shape.AlternativeText.ToLower();
                    switch (alt)
                    {
                        case "name": name = _shape.TextFrame.TextRange.Text; break;
                        case "description": desc = _shape.TextFrame.TextRange.Text; break;
                        case "options": options = _shape.TextFrame.TextRange.Text; break;
                        default:
                            {
                                if (alt.StartsWith("graph;"))
                                {
                                    type = ComponentType.Chart;
                                    shape = _shape;
                                }
                                else if (alt.StartsWith("text;"))
                                {
                                    type = ComponentType.Text;
                                    shape = _shape;
                                }
                                if (alt.StartsWith("table;"))
                                {
                                    type = ComponentType.Table;
                                    shape = _shape;
                                }
                                break;
                            }
                    }
                }
            }
            return Component.CreateComponent(name, desc, options, type, shape);
        }

        private ComponentType toComponentType(string sType)
        {
            switch (sType.Trim().ToLower())
            {
                case "dev-table": return ComponentType.Table;
                case "dev-graph": return ComponentType.Chart;
                case "dev-label": return ComponentType.Text;
                default: return ComponentType.Text;
            }
        }

        #endregion
    }
}
