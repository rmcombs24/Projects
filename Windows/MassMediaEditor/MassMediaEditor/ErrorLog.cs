using System;

namespace MassMediaEditor
{
    class ErrorLog
    {

        public void WriteToLog(string message, string filePath = "")
        {
            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MassMediaEditor\\error.log";

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(userPath, true))
            {
                file.WriteLine(String.Format("({0}) {1}: {2}", DateTime.Now, filePath, message));
            }
        }
    }
}
