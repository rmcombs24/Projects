using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace MassMediaEditor
{
    public class Settings
    {

        public int MediaType { get; set; }
        public string Theme { get; set; }
        public bool AutoSort { get; set; }

        const string filePath = @"C:\Users\Bob\Desktop\Media\config.json";

        public void LoadStartupConfig()
        {

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                Settings s = (Settings)serializer.Deserialize(file, typeof(Settings));

                MediaType = s.MediaType;
                AutoSort = s.AutoSort;
            }
        }

        public void WriteToSettingsConfig(Settings settings)
        {
                JObject jObj = JObject.Parse(File.ReadAllText(filePath));

                jObj["MediaType"] = settings.MediaType;
                jObj["AutoSort"] = settings.AutoSort;

                File.WriteAllText(filePath, jObj.ToString());
        }

        public Settings()
        {

        }
    }
}
