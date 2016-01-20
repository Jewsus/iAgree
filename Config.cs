using System;
using Newtonsoft.Json;
using System.IO;
using TShockAPI;

namespace iAgree
{
    public class Config
    {
        public static string SavePath = Path.Combine(TShock.SavePath, "iAgree.json");

        public string AgreeMessage = "Congratu-fucking-lation you have just managed to read the fucking rules you little shit.";


        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(File.Open(Config.SavePath, FileMode.CreateNew)))
            {
                sw.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        public static Config Read()
        {
            try
            {
                using (StreamReader sr = new StreamReader(File.Open(Config.SavePath, FileMode.Open)))
                {
                    return JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("Error loading config file: " + ex.ToString());
            }
            return new Config();
        }
    }
}
