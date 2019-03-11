using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassMediaEditor
{
    class ErrorLog
    {

        public void WriteToLog(string message, string filePath = "")
        {            
            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\Bob\Desktop\\error.log", true))
            {
                file.WriteLine(String.Format("({0}) {1}: {2}", DateTime.Now, filePath, message));
            }
        }
    }
}
