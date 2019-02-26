using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.Windows;

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
        public uint? Rating { get; set; }
        public String Tags { get; set; }
        public bool isChecked { get; set; }
        
        public void WriteToShellFile(object mediaFile)
        {
            ShellFile shellFile = ShellFile.FromFilePath(((Media)mediaFile).FilePath);

            //shellFile.Properties.System.FileName.Value      = ((Media)mediaFile).FileName; //ToDo: Add FileName Editing later, as it might be better for the Append/Prepend program.
            shellFile.Properties.System.Title.Value         = ((Media)mediaFile).Title;
            shellFile.Properties.System.Subject.Value       = ((Media)mediaFile).Subject;
            shellFile.Properties.System.Comment.Value       = ((Media)mediaFile).Comments;
            shellFile.Properties.System.Rating.Value        = ((Media)mediaFile).Rating;

            if (mediaFile is Picture)
            { 
                String[] authorArray = ((Picture)mediaFile).Authors.Split(',').ToArray();
                SanitizeArray(authorArray);

                shellFile.Properties.System.Author.Value = authorArray;
                shellFile.Properties.System.ApplicationName.Value = ((Picture)mediaFile).ProgramName;
                shellFile.Properties.System.Copyright.Value = ((Picture)mediaFile).Copyright;
                shellFile.Properties.System.DateAcquired.Value = ((Picture)mediaFile).DateAquired;
            }

     //shellFile.Properties.System.DateAccessed.Value = ((Picture)mediaFile).DateTaken;

            else if (mediaFile is Audio) { }
            else if (mediaFile is Video) { }
            
        }
        private void SanitizeArray(String[] itemArray)
        {
            for (int index = 0; index < itemArray.Length; index++)
            {
                itemArray[index] = itemArray[index].Trim();
            }
        }
    }
    
    class Picture : Media
    {
        /* Editable Extened properties for Images (This does not include Camera/Photo settings)
         **** Origin
         * Authors
         * Date Taken
         * Program Name
         * Date Aquired
         * Copyright
         */

        public Picture(){/*Default Constructor*/}

        public Picture(string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath    = filePath;
            Authors     = (file.Properties.System.Author.Value != null) ? String.Join(",", file.Properties.System.Author.Value) : String.Empty;
            
            FileName    = file.Properties.System.FileName.Value;
            Title       = file.Properties.System.Title.Value;
            Subject     = file.Properties.System.Subject.Value;
            Comments    = file.Properties.System.Comment.Value;
            ProgramName = file.Properties.System.ApplicationName.Value;
            Copyright   = file.Properties.System.Copyright.Value;


            DateAquired = file.Properties.System.DateAcquired.Value;
            Rating      = file.Properties.System.Rating.Value;

            DateTaken   = file.Properties.System.DateAccessed.Value;
            Tags        = (file.Properties.System.Keywords.Value != null) ? String.Join(",", file.Properties.System.Keywords.Value) : String.Empty;
        }

        public Dictionary<String, Binding> GenerateBindings()
        {
            var d = new Dictionary<String, Binding>();

            d.Add(String.Empty, new Binding("isChecked"));
            d.Add("File Name", new Binding("FileName"));
            d.Add("Title", new Binding("Title"));
            d.Add("Subject", new Binding("Subject"));
            d.Add("Rating", new Binding("Rating"));
            d.Add("Comments", new Binding("Comments"));
            d.Add("Author", new Binding("Authors"));
            d.Add("Program Name", new Binding("ProgramName"));
            d.Add("Date Taken", new Binding("DateTaken"));
            d.Add("Date Aquired", new Binding("DateAquired"));
            d.Add("Copyright", new Binding("Copyright"));
            d.Add("Tags", new Binding("Tags"));

            return d;
        }

        public string Authors { get; set; }
        public String ProgramName { get; set; }
        public DateTime? DateAquired { get; set; }
        public DateTime? DateTaken { get; set; }
        public String Copyright { get; set; }
    }

    class Audio : Media
    {

    }

    class Video : Media
    {

    }


}
