using System.Windows;
using System.Windows.Documents;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

namespace MassMediaEditor
{
    public partial class ErrorLogWindow : Window
    {
        ErrorLog eLog = new ErrorLog();
        TextRange textRange;

        public ErrorLogWindow()
        {
            InitializeComponent();
            LoadErrorLog();
        }

        private void LoadErrorLog()
        {
            textRange = new TextRange(rtbLogText.Document.ContentStart, rtbLogText.Document.ContentEnd);
            FileStream fileStream = new FileStream(eLog.GetLogPath, FileMode.Open, FileAccess.Read);
            StringBuilder sb = new StringBuilder();

           using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
           {
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    sb.AppendFormat("{0} \r" , line);
                }

                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    textRange.Text = sb.ToString();
                }
                else
                {
                    textRange.Text = "There are currently no entries.";
                }
           }
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxMgr mbMgr = new MessageBoxMgr();
            MessageBoxResult result = mbMgr.CreateNewResult("Are you sure you want to clear the error log?", "Clear Error Log?", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                eLog.ClearErrorLog();
                textRange.Text = "There are currently no entries.";
            }
        }
    }
}
