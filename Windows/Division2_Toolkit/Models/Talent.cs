using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Division2Toolkit
{
    public class Talent
    {
        public string Name { get; set; }

        public string Description { get; set; }

        //Active/requirements later

        public Talent(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
