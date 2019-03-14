using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MassMediaEditor
{
    public class Settings
    {

        public int MediaType { get; set; }
        public string Theme { get; set; }
        public bool AutoSort { get; set; }
        
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MassMediaEditor";

        public void LoadStartupConfig()
        {

            if (!File.Exists(filePath + "\\config.json"))
            {
                Directory.CreateDirectory(filePath);
                File.AppendText(filePath + "\\config.json");
            }

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(filePath + "\\config.json"))
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
