using System;
using System.IO;

namespace MassMediaEditor
{
    public partial class ErrorLog
    {
        public string GetLogPath { get {return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MassMediaEditor\\error.log";} }

        public void WriteToLog(string message, string StackTrace , string filePath = "")
        {
            using (StreamWriter file = new StreamWriter(GetLogPath, true))
            {
                file.WriteLine(String.Format("({0}) {1}: {2} \r Stack Trace:{3} \n", DateTime.Now, filePath, message, StackTrace));
            }
        }

        public void ClearErrorLog()
        {
            File.Create(GetLogPath).Close();
        }
    }
}
