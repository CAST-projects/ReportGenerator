using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace TaskPaneComponents
{
    public class DoDragDrop : Behavior<FrameworkElement>
    {
        Point startPoint;
        bool isPressed = false;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.AddHandler(FrameworkElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(this.AssociatedObject_MouseLeftButtonDown), handledEventsToo: false);
            this.AssociatedObject.AddHandler(FrameworkElement.PreviewMouseMoveEvent, new MouseEventHandler(this.AssociatedObject_MouseMove), handledEventsToo: false);
        }

        void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
            isPressed = true;
        }

        void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            ComponentView draggable = sender as ComponentView;

            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (isPressed && e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                ComponentViewModel compVM = this.AssociatedObject.DataContext as ComponentViewModel;

                if (compVM != null)
                {
                    compVM.DoDragDrop(mousePos.X, mousePos.Y);

                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject("myFormat", compVM);
                    DragDrop.DoDragDrop(draggable, dragData, DragDropEffects.Move);

                    isPressed=false;
                }
            }
        }
    }
}
