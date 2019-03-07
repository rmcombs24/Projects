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

        public MessageBoxResult CompleteMessage (bool completedOperation)
        {

            if (!completedOperation)
            {
                return MessageBox.Show("Operation completed but with errors. Please check the log for more details.", "Operation Complete", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                string tmp1 = string.Format("Commit Complete.", completedOperation);

                return MessageBox.Show(tmp1, tmp1, MessageBoxButton.OK);
            }

        }

        public MessageBoxResult CreateNewResult(string message, string title, MessageBoxButton buttonLayout)
        {
            return MessageBox.Show(message, title, buttonLayout);
        }
    }
}
