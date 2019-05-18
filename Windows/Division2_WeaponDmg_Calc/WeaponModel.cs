using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Division2_WeaponDmg_Calc
{
    class WeaponModel
    {
        public static string Family     { get; set; }
        public static string Make       { get; set; }
        public static string Model      { get; set; }

        public int CritDistanceMin      { get; set; }
        public int CritDistanceMax      { get; set; }
        public int RPM                  { get; set; }
        public int OptimalRange         { get; set; }
        public int MagSize              { get; set; }
        public int ReloadSpeed          { get; set; }
        public double HSMultipler       { get; set; }
        public int NormalizedDmg        { get; set; }
        public int[] DamageRange        { get; set; }
        public bool isExotic            { get; set; }

        public WeaponModel()
        {




        }
    }
}
