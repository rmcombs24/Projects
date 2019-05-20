﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Division2_WeaponDmg_Calc
{
    class WeaponModel
    {
        public string Family { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }

        public int CritDistanceMin { get; set; }
        public int CritDistanceMax { get; set; }
        public int RPM { get; set; }
        public int OptimalRange { get; set; }
        public int MagSize { get; set; }
        public double ReloadSpeed { get; set; }
        public double HSMultipler { get; set; }
        public int NormalizedDmg { get; set; }
        public int[] DamageRange { get; set; }
        public bool isExotic { get; set; }

        public WeaponModel(string family, string make, string model, int critDistanceMin, int critDistanceMax, int rpm, int optimalRange, int magSize, double reloadSpeed, double hsMultipler, int normalizedDmg, string damageRange)
        {

            Family = family;
            Make = make;
            Model = model;

            CritDistanceMin = critDistanceMin;
            CritDistanceMax = critDistanceMax;
            RPM = rpm;
            OptimalRange = optimalRange;
            MagSize = magSize;
            ReloadSpeed = reloadSpeed;
            HSMultipler = hsMultipler;
            NormalizedDmg = normalizedDmg;
            DamageRange = Array.ConvertAll(damageRange.Replace(" - ", ";").Split(';'), int.Parse);
            isExotic = false;
        }

        public static List<WeaponModel> ReadCSV(string fileName)
        {
            string currentFamily = String.Empty;
            string currentMake = String.Empty;
            try
            {
                string path = Path.Combine(Environment.CurrentDirectory, fileName);
                List<WeaponModel> weaponList = new List<WeaponModel>();
                // We change file extension here to make sure it's a .csv file.
                string[] lines = File.ReadAllLines(Path.ChangeExtension(path, ".csv"));

                // lines.Select allows me to project each line as a Person. 
                // This will give me an IEnumerable<Person> back.
                //int index = 0;
                //foreach (string line in lines)
                //
                for (int index = 0; index < lines.Count(); index++)
                {
                    double parse = 0;
                    //index++;
                    string[] data = lines[index].Split(',');

                    if (!String.IsNullOrEmpty(data[0])) { currentFamily = data[0]; }
                    if (!String.IsNullOrEmpty(data[1])) { currentMake = data[1]; }

                    // We return a person with the data in order.
                    weaponList.Add(
                      new WeaponModel(
                        (!String.IsNullOrEmpty(data[0])) ? data[0] : currentFamily,
                        (!String.IsNullOrEmpty(data[1])) ? data[1] : currentMake,
                        data[2],
                        Convert.ToInt32(data[5]),
                        Convert.ToInt32(data[6]),
                        Convert.ToInt32(data[7]),
                        Convert.ToInt32(data[3]),
                        Convert.ToInt32(data[8]),
                        Convert.ToInt32(data[9]),
                       (Double.TryParse(data[10], out parse)) ? parse : 1,
                        Convert.ToInt32(data[12]),
                        data[15]));
                };

                return weaponList;
            }
            catch (Exception e)
            {
                ErrorLog.WriteToLog(e.Message, e.StackTrace);
                throw e;
            }
        }

        public static List<string> GetWeaponFamilies(List<WeaponModel> weaponsList)
        {
            return weaponsList.Select(o => o.Family).Distinct().ToList();
        }

        public static List<string> GetWeaponMakes(List<WeaponModel> weaponsList, string currentFamily)
        {
            return weaponsList.Select(o => o.Make).Distinct().ToList();
        }

        public static List<string> GetWeaponModels(List<WeaponModel> weaponsList, string currentMake)
        {
            return weaponsList.Select(o => o.Model).Distinct().ToList();
        }

        public static List<string> GetWeaponMakesByFamily(List<WeaponModel> weaponsList, string currentFamily)
        {
            return weaponsList.Where(x => x.Family == currentFamily).Select(o => o.Make).Distinct().ToList();
        }

        public static List<string> GetWeaponModelsByMake(List<WeaponModel> weaponsList, string currentMake)
        {
            return weaponsList.Where(x => x.Make == currentMake).Select(o => o.Model).Distinct().ToList();
        }
    }
}