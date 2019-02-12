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

        //ToDo: Add a method that smartly detects the subclass based off of the extension of the filetype.
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

                //shellFile.Properties.System.Author.Value = ((Picture)mediaFile).Author;
                //shellFile.Properties.System.ApplicationName.Value = ((Picture)mediaFile).ApplicationName;
                //shellFile.Properties.System.Copyright.Value = ((Picture)mediaFile).Copyright;
                //shellFile.Properties.System.DateAcquired.Value = ((Media)mediaFile).DateAquired;

            }
            else if (mediaFile is Audio) { }
            else if (mediaFile is Video) { }
            
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
            Authors     = file.Properties.System.Author.Value;

            FileName    = file.Properties.System.FileName.Value;
            Title       = file.Properties.System.Title.Value;
            Subject     = file.Properties.System.Subject.Value;
            Comments    = file.Properties.System.Comment.Value;
            ProgramName = file.Properties.System.ApplicationName.Value;
            Copyright   = file.Properties.System.Copyright.Value;


            DateAquired = file.Properties.System.DateAcquired.Value;
            Rating      = file.Properties.System.Rating.Value;

            //DateTaken   = file.Properties.System.DateAccessed.Value;

            string delim = ";";

            if (file.Properties.System.Keywords.Value != null)
            {
                Tags = file.Properties.System.Keywords.Value.ToList().Aggregate((i, j) => i + delim + j);
            }
            else
            {
                Tags = String.Empty;
            }
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
            d.Add("Author", new Binding("Author"));
            d.Add("Date Aquired", new Binding("DateAquired"));
            d.Add("Copyright", new Binding("Copyright"));
            d.Add("Tags", new Binding("Tags"));

            return d;
        }
        
        public string[] Authors { get; set; }
        public String ProgramName { get; set; }
        public DateTime? DateAquired { get; set; }
        public DateTime DateTaken { get; set; }
        public String Copyright { get; set; }
    }

    class Audio : Media
    {

    }

    class Video : Media
    {

    }

}
