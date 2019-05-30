using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ExcelDataReader;
using System.Text;
using System.Threading.Tasks;

namespace Division2Toolkit
{
    class Exotic
    {
        public string Name { get; set; }

        public Talent HolsteredTalent { get; set; }
        public Talent ActiveTalent_1 { get; set; }
        public Talent ActiveTalent_2 { get; set; }


        public static List<Exotic> ReadExoticXLSX(string filename)
        {
            string path = Path.Combine(Environment.CurrentDirectory, filename);
            DataSet dsGearTable = new DataSet();
            List<Exotic> gearList = new List<Exotic>();

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    dsGearTable = reader.AsDataSet();

                    for (int tableIndex = 0; tableIndex < dsGearTable.Tables.Count; tableIndex++)
                    {                        
                        foreach (DataRow currentRow in dsGearTable.Tables[tableIndex].Rows)
                        {
                            Exotic currentExotic = new Exotic();
                            Talent currentTalent = new Talent();

                            //I'll refactor this later. I'm literally too tired to figure out the pattern recognition
                            currentExotic.Name = currentRow.ItemArray[0].ToString();

                            currentTalent.Name = currentRow.ItemArray[1].ToString();
                            currentTalent.Description = currentRow.ItemArray[2].ToString();

                            currentExotic.ActiveTalent_1 = currentTalent;

                            currentTalent.Name = currentRow.ItemArray[3].ToString();
                            currentTalent.Description = currentRow.ItemArray[4].ToString();

                            currentExotic.ActiveTalent_2 = currentTalent;

                            currentTalent.Name = currentRow.ItemArray[5].ToString();
                            currentTalent.Description = currentRow.ItemArray[6].ToString();

                            currentExotic.HolsteredTalent = currentTalent;

                            gearList.Add(currentExotic);
                        }
                    }
                }
            }

            return gearList;
        }
    }
}

