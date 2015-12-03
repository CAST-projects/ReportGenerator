using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TaskPaneComponents
{
    public class CommandViewModel : ViewModelBase
    {
        public CommandViewModel(ICommand command)
        {
            this.Command = command;
        }

        public ICommand Command { get; private set; }
    }
}
