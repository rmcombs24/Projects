using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Division2Toolkit.ViewModels
{
    class GearPercentileViewModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Gear Percentiles";
            }
        }
    }
}
