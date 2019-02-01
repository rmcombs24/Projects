using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassMediaEditor
{
    class Media
    {
        /* These are shared by all media types
        **** Description
        * Title 
        * Subject
        * Rating
        * Tags
        * Comments
        */
        
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public String Title { get; set; }
        public String Subject { get; set; }
        public String Comments { get; set; }
        public double Rating { get; set; }
        public List<String> Tags { get; set; }
    }

    class Pictures : Media
    {
        /* Editable Extened properties for Images (This does not include Camera/Photo settings)
         **** Origin
         * Authors
         * Date Taken
         * Program Name
         * Date Aquired
         * Copyright
         */

        public List<string> Authors { get; set; }
        public String ProgramName { get; set; }
        public DateTime DateAquired { get; set; }
        public DateTime DateTaken { get; set; }
        public double Copyright { get; set; }
    }

    class Audio : Media
    {

    }

    class Video : Media
    {

    }
}
