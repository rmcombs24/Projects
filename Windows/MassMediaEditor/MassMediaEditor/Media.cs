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

        public string FileName  { get; set; }
        public string FilePath  { get; set; }
        public String Title     { get; set; }
        public String Subtitle  { get; set; }
        public String Subject   { get; set; }
        public String Comments  { get; set; }
        public uint? Rating     { get; set; }
        public String Tags      { get; set; }
        public bool isChecked   { get; set; }
        
        public void WriteToShellFile(object mediaFile)
        {
            ShellFile shellFile = ShellFile.FromFilePath(((Media)mediaFile).FilePath);
            
            //shellFile.Properties.System.FileName.Value      = ((Media)mediaFile).FileName; //ToDo: Add FileName Editing later, as it might be better for the Append/Prepend program.
            shellFile.Properties.System.Title.Value         = ((Media)mediaFile).Title;
            shellFile.Properties.System.Subject.Value       = ((Media)mediaFile).Subject;
            shellFile.Properties.System.Comment.Value       = ((Media)mediaFile).Comments;
            shellFile.Properties.System.Rating.Value        = ((Media)mediaFile).Rating;

            String[] tagsArray                              = ((Media)mediaFile).Tags.Split(',').ToArray();
            SanitizeArray(tagsArray);

            shellFile.Properties.System.Keywords.Value      = tagsArray;

            if (mediaFile is Picture)
            { 
                String[] authorArray                                    = ((Picture)mediaFile).Authors.Split(',').ToArray(); 
                SanitizeArray(authorArray);

                shellFile.Properties.System.Author.Value                = authorArray;
                shellFile.Properties.System.ApplicationName.Value       = ((Picture)mediaFile).ProgramName;
                shellFile.Properties.System.Copyright.Value             = ((Picture)mediaFile).Copyright;
                shellFile.Properties.System.DateAcquired.Value          = ((Picture)mediaFile).DateAcquired;
                shellFile.Properties.System.Photo.DateTaken.Value       = ((Picture)mediaFile).DateTaken;
            }

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

        public Dictionary<String, Binding> GenerateBindings(Type MediaType)
        {
            var d = new Dictionary<String, Binding>();

            d.Add(String.Empty, new Binding("isChecked"));
            d.Add("File Name", new Binding("FileName"));
            d.Add("Title", new Binding("Title"));
            d.Add("Copyright", new Binding("Copyright"));
            d.Add("Tags", new Binding("Tags"));
            d.Add("Rating", new Binding("Rating"));
            d.Add("Comments", new Binding("Comments"));

            if (MediaType.Name == "Picture")
            {
                d.Add("Subject", new Binding("Subject"));
                d.Add("Author", new Binding("Authors"));
                d.Add("Program Name", new Binding("ProgramName"));
                d.Add("Date Taken", new Binding("DateTaken"));
                d.Add("Date Acquired", new Binding("DateAcquired"));
            } /*
            else if (Media is Audio)
            {

            }
            else if (Media is Video)
            {

            } */

            return d;
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

            FilePath        = filePath;
            Authors         = (file.Properties.System.Author.Value != null) ? String.Join(",", file.Properties.System.Author.Value) : String.Empty;
            Tags            = (file.Properties.System.Keywords.Value != null) ? String.Join(",", file.Properties.System.Keywords.Value) : String.Empty;

            FileName        = file.Properties.System.FileName.Value;
            Title           = file.Properties.System.Title.Value;
            Subject         = file.Properties.System.Subject.Value;
            Comments        = file.Properties.System.Comment.Value;
            ProgramName     = file.Properties.System.ApplicationName.Value;
            Copyright       = file.Properties.System.Copyright.Value;


            DateAcquired    = file.Properties.System.DateAcquired.Value;
            Rating          = file.Properties.System.Rating.Value;

            DateTaken       = file.Properties.System.Photo.DateTaken.Value;

        }

        public String Authors           { get; set; }
        public String ProgramName       { get; set; }
        public DateTime? DateAcquired   { get; set; }
        public DateTime? DateTaken      { get; set; }
        public String Copyright         { get; set; }
    }

    class Audio : Media
    {
        /* Editable Extened properties for Audio Files
         **** Contributing Artists
         * Album Artist
         * Album
         * Track Number
         * Genre
         * Copyright
         **** Content (There are a few, but I am not sure if I wish to add them)
         * Composers
         * BPM
         */

        public String ContributingArtists   { get; set; }
        public String AlbumArtist           { get; set; }
        public String Album                 { get; set; }
        public uint? TrackNumber            { get; set; }
        public String Copyright             { get; set; }
        public String Creator               { get; set; }
        public String Publisher             { get; set; }
        public String Genre                 { get; set; }
        public String Composers             { get; set; }
        public String BPM                   { get; set; }

        public Audio(string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath = filePath;
            FileName                = file.Properties.System.FileName.Value;

            Composers               = (file.Properties.System.Music.Composer.Value != null) ? String.Join(",", file.Properties.System.Music.Composer.Value) : String.Empty;
            ContributingArtists     = (file.Properties.System.Music.Artist.Value != null) ? String.Join(",", file.Properties.System.Music.Artist.Value) : String.Empty;
            Genre                   = (file.Properties.System.Music.Genre.Value != null) ? String.Join(",", file.Properties.System.Music.Genre.Value) : String.Empty;
            Tags                    = (file.Properties.System.Keywords.Value != null) ? String.Join(",", file.Properties.System.Keywords.Value) : String.Empty;

            Comments                = file.Properties.System.Comment.Value;
            Copyright               = file.Properties.System.Copyright.Value;
            Rating                  = file.Properties.System.Rating.Value;

            AlbumArtist             = file.Properties.System.Music.AlbumArtist.Value;
            Album                   = file.Properties.System.Music.AlbumTitle.Value;
            TrackNumber             = file.Properties.System.Music.TrackNumber.Value;

            Publisher               = file.Properties.System.Media.Publisher.Value;
            //Creator                 = file.Properties.System.Media.Creator.Value; WTF WHERE DID THIS GO?!
        }

        public Audio() {/*Default Constructor*/}

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
            d.Add("Copyright", new Binding("Copyright"));
            d.Add("Tags", new Binding("Tags"));

            d.Add("Date Taken", new Binding("DateTaken"));
            d.Add("Date Acquired", new Binding("DateAcquired"));
            return d;
        }
    }

    class Video : Media
    {

    }

}
