using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using Microsoft.WindowsAPICodePack.Shell;

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

        public bool WriteToShellFile(Media mediaFile, bool sort)
        {
            bool hasErrors = false;
            char[] seperators = { ',', ';', '|', '/', '\\', ':' };

            ShellFile shellFile = ShellFile.FromFilePath(((Media)mediaFile).FilePath);

            try
            {
                //shellFile.Properties.System.FileName.Value      = ((Media)mediaFile).FileName; //ToDo: Add FileName Editing later, as it might be better for the Append/Prepend program.
                shellFile.Properties.System.Title.Value     = ((Media)mediaFile).Title;
                shellFile.Properties.System.Subject.Value   = ((Media)mediaFile).Subject;
                shellFile.Properties.System.Comment.Value   = ((Media)mediaFile).Comments;
                shellFile.Properties.System.Media.Subtitle.Value = ((Media)mediaFile).Subtitle;

                if (((Media)mediaFile).Rating == 0)
                {
                    shellFile.Properties.System.Rating.Value = null;
                }
                else if (((Media)mediaFile).Rating == 100)
                {
                    shellFile.Properties.System.Rating.Value = 99;
                }
                else { shellFile.Properties.System.Rating.Value = ((Media)mediaFile).Rating; }
                
                shellFile.Properties.System.Keywords.Value = (String.IsNullOrEmpty(((Media)mediaFile).Tags.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0])) ? null : SanitizeArray(((Media)mediaFile).Tags.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort);

                if (mediaFile is Picture)
                {                   
                    shellFile.Properties.System.Author.Value = (String.IsNullOrEmpty(((Picture)mediaFile).Authors.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0])) ? null : SanitizeArray(((Picture)mediaFile).Authors.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort); ;
                    shellFile.Properties.System.ApplicationName.Value = ((Picture)mediaFile).ProgramName;
                    shellFile.Properties.System.Copyright.Value = ((Picture)mediaFile).Copyright;
                    shellFile.Properties.System.DateAcquired.Value = ((Picture)mediaFile).DateAcquired;
                    shellFile.Properties.System.Photo.DateTaken.Value = ((Picture)mediaFile).DateTaken;
                }
                else if (mediaFile is Audio)
                {
                    //shellFile.Properties.System.Copyright.Value             = ((Audio)mediaFile).Copyright;

                    shellFile.Properties.System.Music.Composer.Value        = (String.IsNullOrEmpty(((Audio)mediaFile).Composers.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0]))           ?  null  : SanitizeArray(((Audio)mediaFile).Composers.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort);
                    shellFile.Properties.System.Music.Artist.Value          = (String.IsNullOrEmpty(((Audio)mediaFile).ContributingArtists.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0])) ?  null  : SanitizeArray(((Audio)mediaFile).ContributingArtists.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort);
                    shellFile.Properties.System.Music.Genre.Value           = (String.IsNullOrEmpty(((Audio)mediaFile).Genre.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0]))               ?  null  : SanitizeArray(((Audio)mediaFile).Genre.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort);
                    shellFile.Properties.System.Music.AlbumArtist.Value     = ((Audio)mediaFile).AlbumArtist;
                    shellFile.Properties.System.Music.AlbumTitle.Value      = ((Audio)mediaFile).Album;
                    shellFile.Properties.System.Music.TrackNumber.Value     = ((Audio)mediaFile).TrackNumber;
                    shellFile.Properties.System.Music.BeatsPerMinute.Value  = ((Audio)mediaFile).BPM;

                    shellFile.Properties.System.Media.Publisher.Value       = ((Audio)mediaFile).Publisher;
                }
                else if (mediaFile is Video)
                {
                    //shellFile.Properties.System.Copyright.Value = String.IsNullOrEmpty(((Video)mediaFile).Copyright) ? ((Video)mediaFile).Copyright : String.Empty;
                    
                    shellFile.Properties.System.Music.Artist.Value =    (String.IsNullOrEmpty(((Video)mediaFile).ContributingArtists.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0])   ?  null : SanitizeArray(((Video)mediaFile).ContributingArtists.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort));
                    shellFile.Properties.System.Music.Genre.Value =     (String.IsNullOrEmpty(((Video)mediaFile).Genre.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0])                 ?  null : SanitizeArray(((Video)mediaFile).Genre.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort));

                    shellFile.Properties.System.Media.Writer.Value =    (String.IsNullOrEmpty(((Video)mediaFile).Writers.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0])               ?  null : SanitizeArray(((Video)mediaFile).Writers.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort));
                    shellFile.Properties.System.Media.Producer.Value =  (String.IsNullOrEmpty(((Video)mediaFile).Producers.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0])             ?  null : SanitizeArray(((Video)mediaFile).Producers.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort));
                    shellFile.Properties.System.Video.Director.Value =  (String.IsNullOrEmpty(((Video)mediaFile).Directors.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray()[0])             ?  null : SanitizeArray(((Video)mediaFile).Directors.Split(seperators, StringSplitOptions.RemoveEmptyEntries).ToArray(), sort));

                    shellFile.Properties.System.Media.Publisher.Value       = ((Video)mediaFile).Publisher;
                    shellFile.Properties.System.Media.AuthorUrl.Value       = ((Video)mediaFile).AuthorURL;
                    shellFile.Properties.System.Media.PromotionUrl.Value    = ((Video)mediaFile).PromoURL;
                    shellFile.Properties.System.Media.Year.Value            = ((Video)mediaFile).Year;
                }
            }
            catch (Exception e)
            {
                new ErrorLog().WriteToLog(e.Message, mediaFile.FilePath);
                hasErrors = true;    
            }

            return !hasErrors;
        }

        private string[] SanitizeArray(String[] itemArray, bool sort)
        {
            for (int index = 0; index < itemArray.Length; index++)
            {
                itemArray[index] = itemArray[index].Trim();
            }

            //Check the config to see if sorting is enabled.
            if (sort)
            {
                Array.Sort(itemArray, StringComparer.InvariantCulture);
            }

            return itemArray;
        }

        public Dictionary<String, Binding> GenerateBindings <T> (Type MediaType)
        {
            var d = new Dictionary<String, Binding>();

            d.Add(String.Empty, new Binding("isChecked"));
            d.Add("File Name",  new Binding("FileName"));
            d.Add("Title",      new Binding("Title"));

            if (false) //Overwrite readonly setting
            {
                d.Add("Copyright", new Binding("Copyright"));
            }

            d.Add("Rating",     new Binding("Rating"));
            d.Add("Comments",   new Binding("Comments"));

            if (MediaType.Name == "Picture")
            {
                d.Add("Subject",        new Binding("Subject"));
                d.Add("Author",         new Binding("Authors"));
                d.Add("Program Name",   new Binding("ProgramName"));
                d.Add("Date Taken",     new Binding("DateTaken"));
                d.Add("Date Acquired",  new Binding("DateAcquired"));
                d.Add("Tags",           new Binding("Tags"));

            }
            else if (MediaType.Name == "Audio")
            {
                d.Add("Subtitle",               new Binding("Subtitle"));
                d.Add("Contributing Artists",   new Binding("ContributingArtists"));
                d.Add("Album Artist",           new Binding("AlbumArtist"));
                d.Add("Album",                  new Binding("Album"));
                d.Add("Track Number",           new Binding("TrackNumber"));
                d.Add("Genre",                  new Binding("Genre"));
                d.Add("Composers",              new Binding("Composers"));
                d.Add("BPM",                    new Binding("BPM"));
                d.Add("Author URL",             new Binding("AuthorURL"));
                d.Add("Publisher",              new Binding("Publisher"));
            }
            else if (MediaType.Name == "Video")
            {
                d.Add("Tags",                   new Binding("Tags"));
                d.Add("Subtitle",               new Binding("Subtitle"));
                d.Add("Contributing Artists",   new Binding("ContributingArtists"));
                d.Add("Publisher",              new Binding("Publisher"));
                d.Add("Directors",              new Binding("Directors"));
                d.Add("Writers",                new Binding("Writers"));
                d.Add("Genre",                  new Binding("Genre"));
                d.Add("Media Created",          new Binding("MediaCreated"));
                d.Add("Year",                   new Binding("Year"));
                d.Add("Producers",              new Binding("Producers"));
                d.Add("Author URL",             new Binding("AuthorURL"));
                d.Add("Promotional URL",        new Binding("PromoURL"));
            }

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

            Composers               = (file.Properties.System.Music.Composer.Value  != null) ? String.Join(",", file.Properties.System.Music.Composer.Value)    : String.Empty;
            ContributingArtists     = (file.Properties.System.Music.Artist.Value    != null) ? String.Join(",", file.Properties.System.Music.Artist.Value)      : String.Empty;
            Genre                   = (file.Properties.System.Music.Genre.Value     != null) ? String.Join(",", file.Properties.System.Music.Genre.Value)       : String.Empty;
            Tags                    = (file.Properties.System.Keywords.Value        != null) ? String.Join(",", file.Properties.System.Keywords.Value)          : String.Empty;

            Comments                = file.Properties.System.Comment.Value;
            Copyright               = file.Properties.System.Copyright.Value;
            Rating                  = file.Properties.System.Rating.Value;
            Title                   = file.Properties.System.Title.Value;
            

            AlbumArtist             = file.Properties.System.Music.AlbumArtist.Value;
            Album                   = file.Properties.System.Music.AlbumTitle.Value;
            TrackNumber             = file.Properties.System.Music.TrackNumber.Value;
            BPM                     = file.Properties.System.Music.BeatsPerMinute.Value;

            Publisher               = file.Properties.System.Media.Publisher.Value;
            Subtitle                = file.Properties.System.Media.Subtitle.Value;
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
        /* Editable Extened properties for Audio Files
            -----
            Media
            -----
            Contributing Artists
            Year
            Genre

            ------
            Origin
            ------
            Directors
            Producers
            Writers
            Publisher
            Content Provider
            Media Created
            Author URL
            Promotion URL
         */

        public String ContributingArtists { get; set; }
        public uint? Year { get; set; }
        public String Genre { get; set; }
        public String Directors { get; set; }
        public String Writers { get; set; }
        public String Publisher { get; set; }
        public DateTime? MediaCreated { get; set; }
        public String Producers { get; set; }
        public String AuthorURL { get; set; } 
        public String PromoURL { get; set; }


        public String Copyright { get; set; }

        public Video (string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath = filePath;
            FileName = file.Properties.System.FileName.Value;

            ContributingArtists =   (file.Properties.System.Music.Artist.Value != null)     ? String.Join(",", file.Properties.System.Music.Artist.Value)   : String.Empty;
            Directors =             (file.Properties.System.Video.Director.Value != null)   ? String.Join(",", file.Properties.System.Video.Director.Value) : String.Empty;
            Producers =             (file.Properties.System.Media.Producer.Value != null)   ? String.Join(",", file.Properties.System.Media.Producer.Value) : String.Empty;
            Writers =               (file.Properties.System.Media.Writer.Value != null)     ? String.Join(",", file.Properties.System.Media.Writer.Value)   : String.Empty;
            Genre =                 (file.Properties.System.Music.Genre.Value != null)      ? String.Join(",", file.Properties.System.Music.Genre.Value)    : String.Empty;
            Tags =                  (file.Properties.System.Keywords.Value != null)         ? String.Join(",", file.Properties.System.Keywords.Value)       : String.Empty;

            MediaCreated = file.Properties.System.DateCreated.Value;

            Comments = file.Properties.System.Comment.Value;
            Copyright = file.Properties.System.Copyright.Value;
            Rating = file.Properties.System.Rating.Value;
            Title = file.Properties.System.Title.Value;

            AuthorURL = file.Properties.System.Media.AuthorUrl.Value;
            PromoURL = file.Properties.System.Media.PromotionUrl.Value;

            Publisher = file.Properties.System.Media.Publisher.Value;
            Year = file.Properties.System.Media.Year.Value;
            Publisher = file.Properties.System.Media.Publisher.Value;
            Subtitle = file.Properties.System.Media.Subtitle.Value;
            //Creator                 = file.Properties.System.Media.Creator.Value; WTF WHERE DID THIS GO?!
        }

        public Video () {/*Default Constructor*/}
    }

    public enum MediaType
    {
        Audio,
        Video,
        Pictures
    }
}