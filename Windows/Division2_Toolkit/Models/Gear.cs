using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;

namespace Division2Toolkit
{
    class Gear
    {
        public List<GearAttribute> gearAttributes { get; set; }
        public string gearType { get; set; }

        public bool SetPiece { get; set; }

        public static List<Gear> ReadGearXLSX(string filename)
        {
            string path = Path.Combine(Environment.CurrentDirectory, filename);
            DataSet dsGearTable = new DataSet();
            List<Gear> gearList = new List<Gear>();

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    dsGearTable = reader.AsDataSet();

                    for (int tableIndex = 0; tableIndex < dsGearTable.Tables.Count; tableIndex++)
                    {
                        Gear newGear = new Gear()
                        {
                            gearType = dsGearTable.Tables[tableIndex].TableName,
                            gearAttributes = new List<GearAttribute>()
                        };

                        int currentCol = 0, minCol = 0, maxCol = 0, maxSetCol = 0, typeCol = 0;

                        //Grab the headers
                        foreach (string headerCol in dsGearTable.Tables[tableIndex].Rows[0].ItemArray)
                        {
                            if (headerCol == "Min") { minCol = currentCol; }
                            else if (headerCol == "Max") { maxCol = currentCol; }
                            else if (headerCol == "MaxSet") { maxSetCol = currentCol; }
                            else if (headerCol == "Type") { typeCol = currentCol; }
                            currentCol++;
                        }

                        //Start at 1, because the 0th row is headers
                        for (int rowIndex = 1; rowIndex < dsGearTable.Tables[tableIndex].Rows.Count; rowIndex++)
                        {
                            GearAttribute gearAttribute = new GearAttribute();

                            //Grab the type, min, max and make an attribute.
                            for (int itemIndex = 0; itemIndex < dsGearTable.Tables[tableIndex].Rows[rowIndex].ItemArray.Length; itemIndex++)
                            {
                                if (itemIndex == typeCol) { gearAttribute.AttributeName = dsGearTable.Tables[tableIndex].Rows[rowIndex].ItemArray[itemIndex].ToString(); }
                                else if (itemIndex == minCol) { gearAttribute.minRoll = Convert.ToDouble(dsGearTable.Tables[tableIndex].Rows[rowIndex].ItemArray[itemIndex]); }
                                else if (itemIndex == maxCol) { gearAttribute.maxRoll = Convert.ToDouble(dsGearTable.Tables[tableIndex].Rows[rowIndex].ItemArray[itemIndex]); }
                                else if (itemIndex == maxSetCol) { gearAttribute.setMaxRoll = Convert.ToDouble(dsGearTable.Tables[tableIndex].Rows[rowIndex].ItemArray[itemIndex]); }
                            }

                            newGear.gearAttributes.Add(gearAttribute);
                        }

                        //add the piece of gear to the list.
                        gearList.Add(newGear);
                    }
                }
            }

            return gearList;
        }
    }

    class GearAttribute
    {
        public double minRoll { get; set; }
        public double avgRoll { get; set; }
        public double goodRoll { get; set; }
        public double betterRoll { get; set; }
        public double outstandingRoll { get; set; }
        public double maxRoll { get; set; }
        public double setMaxRoll { get; set; }
        public string AttributeName { get; set; }


        public GearAttribute(string attributeName, double min, double average, double good, double better, double outstanding, double max, double setMax)
        {
            AttributeName = attributeName;
            minRoll = min;
            avgRoll = average;
            goodRoll = good;
            betterRoll = better;
            outstandingRoll = outstanding;
            maxRoll = max;
            setMaxRoll = setMax;
        }

        public GearAttribute(string attributeName, double min, double max, double setMax)
        {
            AttributeName = attributeName;
            minRoll = min;
            maxRoll = max;
            setMaxRoll = setMax;
        }

        public GearAttribute(string attributeName)
        {
            AttributeName = attributeName;
            minRoll = 0;
            maxRoll = 0;
        }

        public GearAttribute() { }
    }
}