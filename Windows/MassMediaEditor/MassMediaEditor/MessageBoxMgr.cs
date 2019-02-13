using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MassMediaEditor
{
    class MessageBoxMgr
    {

        public MessageBoxResult ItemsRequiredMessage ()
        {
            return MessageBox.Show("At least one item must be selected to edit.", "Items Required", MessageBoxButton.OK);
        }

        public MessageBoxResult CompleteMessage (string completedOperation)
        {
            return MessageBox.Show(String.Format("{0} Complete.", completedOperation), String.Format( "{0) Completed", completedOperation), MessageBoxButton.OK);
        }

        public MessageBoxResult CreateNewResult(string message, string title, MessageBoxButton buttonLayout)
        {
            return MessageBox.Show(message, title, buttonLayout);
        }
    }
}
