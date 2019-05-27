using System;
using System.IO;

namespace Division2Toolkit
{
    public partial class ErrorLog
    {
        public static string GetLogPath(){ return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Division2_Toolkit\\error.log"; }

        public static void WriteToLog(string message, string StackTrace , string filePath = "")
        {
            using (StreamWriter file = new StreamWriter(GetLogPath(), true))
            {
                file.WriteLine(String.Format("({0}) {1}: {2} \r Stack Trace:{3} \n", DateTime.Now, filePath, message, StackTrace));
            }
        }

        public void ClearErrorLog()
        {
            File.Create(GetLogPath()).Close();
        }
    }
}
