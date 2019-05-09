using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MassMediaEditor
{
    public class Settings
    {
        public MediaType MediaType { get; set; }
        private static MediaType _MediaType { get; set; }
        public string Theme { get; set; }
        public bool AutoSort { get; set; }
        private static bool _AutoSort { get; set; }

        static string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MassMediaEditor";

        public static void LoadStartupConfig()
        {
            if (!File.Exists(filePath + "\\config.json"))
            {
               Directory.CreateDirectory(filePath);

               Settings newSettings = new Settings() 
                {
                    MediaType = 0,
                    AutoSort = false,
                    Theme = String.Empty
                };

                using (StreamWriter file = File.AppendText(filePath + "\\config.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, newSettings);
                }
            }
            else
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(filePath + "\\config.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Settings s = (Settings)serializer.Deserialize(file, typeof(Settings));

                    _MediaType = s.MediaType;
                    _AutoSort = s.AutoSort;
                }
            }
        }

        public static void WriteToSettingsConfig(Settings settings)
        {
            string json = File.ReadAllText(filePath + "\\config.json");
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj["MediaType"] = settings.MediaType.ToString();
            jsonObj["AutoSort"] = settings.AutoSort;

            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(filePath + "\\config.json", output);
        }

        public static bool CanSort()
        {
            return _AutoSort;
        }

        public static MediaType DefaultMediaType()
        {
            return _MediaType;
        }
    }
}
