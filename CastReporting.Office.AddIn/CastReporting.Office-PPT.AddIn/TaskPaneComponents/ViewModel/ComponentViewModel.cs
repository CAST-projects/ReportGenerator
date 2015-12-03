using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ComponentRepository;

namespace TaskPaneComponents
{
    public delegate void ComponentViewModelDragEventHandler(object sender, DragEventArgs e);

    public class ComponentViewModel : ViewModelBase
    {
        public event ComponentViewModelDragEventHandler ReportDragEvent;

        #region Fields

        ImageSource image;
        Component component;

        #endregion

        #region Constructors

        public ComponentViewModel() { }

        public ComponentViewModel(Component component)
        {
            if (component == null)
                throw new ArgumentNullException("Component");

            this.component = component;
        }

        #endregion

        #region Properties

        public override string DisplayName
        {
            get
            {
                return component.DisplayName;
            }
            //set
            //{
            //    base.DisplayName = value;
            //    base.OnPropertyChanged(Consts.DISPLAY_NAME_PN);
            //}
        }

        public override string Description
        {
            get
            {
                return component.Description;
            }
            //set
            //{
            //    base.Description = value;
            //    base.OnPropertyChanged(Consts.DESCRIPTION_PN);
            //}
        }

        public string ComponentType
        {
            get { return component.CompoType.ToString(); }
        }

        public string Options
        {
            get { return component.Options; }
        }

        public ComponentType CompoType
        {
            get { return component.CompoType; }
        }

        public ImageSource Image
        {
            get
            {
                if (image == null)
                {
                    component.Shape.Copy();
                    if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Bitmap))
                    {
                        image = BinaryStructConverter.ImageFromClipboardDib();
                    }
                }

                return image;
            }
        }

        #endregion

        public void DoDragDrop(double x, double y)
        {
            ComponentViewModelDragEventHandler handler = this.ReportDragEvent;
            if (handler != null)
                handler(this, new DragEventArgs(x, y, component));
        }
    }
}
