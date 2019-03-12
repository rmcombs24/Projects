using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace MassMediaEditor
{
    class Settings
    {

        public string Media_Type { get; set; }
        public string Theme { get; set; }
        public bool AutoSort { get; set; }

        const string filePath = "C:\\Users\\Bob\\Desktop\\config.json";

        private void LoadStartupConfig()
        {

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                //settings = (Settings)serializer.Deserialize(file, typeof(Settings));
            }
        }

        public void WriteToSettingsConfig()
        {

        }
    }
}
