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
        public Talent ActiveTalent { get; set; }
        public Talent PassiveTalent { get; set; }


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

                            //I'll refactor this later. I'm literally too tired to figure out the pattern recognition
                            currentExotic.Name = currentRow.ItemArray[0].ToString();
                            currentExotic.ActiveTalent = new Talent(currentRow.ItemArray[1].ToString(), currentRow.ItemArray[2].ToString());
                            currentExotic.PassiveTalent = new Talent(currentRow.ItemArray[3].ToString(), currentRow.ItemArray[4].ToString());
                            currentExotic.HolsteredTalent = new Talent(currentRow.ItemArray[5].ToString(), currentRow.ItemArray[6].ToString());

                            gearList.Add(currentExotic);
                        }
                    }
                }
            }

            return gearList;
        }
    }
}

