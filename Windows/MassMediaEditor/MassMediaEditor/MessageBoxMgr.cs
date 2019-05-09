using System.Windows;

namespace MassMediaEditor
{
    public static class MessageBoxMgr
    {
        public static MessageBoxResult ItemsRequiredMessage ()
        {
            return MessageBox.Show("At least one item must be selected to continue.", "Items Required", MessageBoxButton.OK);
        }

        public static MessageBoxResult CompleteMessage (bool completedOperation)
        {
            if (!completedOperation)
            {
                return MessageBox.Show("Operation completed but with errors. Please check the log for more details.", "Operation Complete", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                string commitMsg = string.Format("Commit Complete.", completedOperation);

                return MessageBox.Show(commitMsg, commitMsg, MessageBoxButton.OK);
            }
        }

        public static MessageBoxResult CreateNewResult(string message, string title, MessageBoxButton buttonLayout)
        {
            return MessageBox.Show(message, title, buttonLayout);
        }
    }
}
