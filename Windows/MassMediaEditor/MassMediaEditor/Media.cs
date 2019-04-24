using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using Microsoft.WindowsAPICodePack.Shell;
using System.Windows;

namespace MassMediaEditor
{
    class Media
    {

        /* Editable extened properties for Media

            ----- Description -----
            Title, Subject, Rating, Tags, Comments
        */

        //I gotta refactor this whole thing. Nested classes based on sections of attributes and all that, but this will work for now
        #region Media Properties
        public bool isChecked { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public String Title { get; set; }
        public String Subtitle { get; set; }
        public String Subject { get; set; }
        public String Comments { get; set; }
        public uint? Rating { get; set; }
        public String[] Tags { get; set; }
        public String _Tags { get { return (Tags == null) ? String.Empty : String.Join(";", Tags); } }
        public String[] ContributingArtists { get; set; }
        public String _ContributingArtists { get { return (ContributingArtists == null) ? String.Empty : String.Join(";", ContributingArtists); } }
        public String[] Genre { get; set; }
        public String _Genre { get { return (Genre == null) ? String.Empty : String.Join(";", Genre); } }
        public String AuthorURL { get; set; }
        public String Publisher { get; set; }
        #endregion

        public bool WriteToShellFile(Media mediaFile, bool sort)
        {
            bool hasErrors = false;

            ShellFile shellFile = ShellFile.FromFilePath(mediaFile.FilePath);

            try
            {
                //shellFile.Properties.System.FileName.Value  = (mediaFile.FileName; //ToDo: Add FileName Editing later, as it might be better for the Append/Prepend program.
                shellFile.Properties.System.Title.Value     = mediaFile.Title;
                shellFile.Properties.System.Subject.Value   = mediaFile.Subject;
                shellFile.Properties.System.Comment.Value   = mediaFile.Comments;
                if (!(mediaFile is Audio)) { shellFile.Properties.System.Keywords.Value = mediaFile.Tags ?? Array.Empty<string>(); }

                if (mediaFile.Subtitle != null) { shellFile.Properties.System.Media.Subtitle.Value = mediaFile.Subtitle; }

                if (mediaFile.Rating == 0) { shellFile.Properties.System.Rating.Value = null; }
                else if (mediaFile.Rating == 100) { shellFile.Properties.System.Rating.Value = 99; }
                else { shellFile.Properties.System.Rating.Value = mediaFile.Rating; }
            
                if (mediaFile is Picture)
                {
                    //shellFile.Properties.System.Copyright.Value         = ((Picture)mediaFile).Copyright;
                    shellFile.Properties.System.Author.Value            = ((Picture)mediaFile).Authors ?? Array.Empty<string>();
                    shellFile.Properties.System.ApplicationName.Value   = ((Picture)mediaFile).ProgramName;
                    shellFile.Properties.System.DateAcquired.Value      = (((Picture)mediaFile).DateAcquired == DateTime.MinValue) ? null : ((Picture)mediaFile).DateAcquired;
                    shellFile.Properties.System.Photo.DateTaken.Value   = (((Picture)mediaFile).DateTaken == DateTime.MinValue) ? null : ((Picture)mediaFile).DateTaken;
                }
                else if (mediaFile is Audio || mediaFile is Video)
                {
                    if (mediaFile is Video)
                    {
                        //shellFile.Properties.System.DateCreated.Value           = (((Video)mediaFile).MediaCreated != DateTime.MinValue) ? ((Video)mediaFile).MediaCreated : null;
                        shellFile.Properties.System.Media.Writer.Value          = ((Video)mediaFile).Writers ?? Array.Empty<string>();
                        shellFile.Properties.System.Media.Producer.Value        = ((Video)mediaFile).Producers ?? Array.Empty<string>();
                        shellFile.Properties.System.Video.Director.Value        = ((Video)mediaFile).Directors ?? Array.Empty<string>();
                        shellFile.Properties.System.Media.PromotionUrl.Value    = ((Video)mediaFile).PromoURL;
                        shellFile.Properties.System.Media.Year.Value            = ((Video)mediaFile).Year;
                    }
                    else if (mediaFile is Audio)
                    {
                        shellFile.Properties.System.Music.AlbumArtist.Value     = ((Audio)mediaFile).AlbumArtist;
                        shellFile.Properties.System.Music.AlbumTitle.Value      = ((Audio)mediaFile).Album;
                        shellFile.Properties.System.Music.TrackNumber.Value     = ((Audio)mediaFile).TrackNumber;
                        shellFile.Properties.System.Music.BeatsPerMinute.Value  = ((Audio)mediaFile).BPM;
                        shellFile.Properties.System.Music.Composer.Value        = ((Audio)mediaFile).Composers ?? Array.Empty<string>();
                    }

                    shellFile.Properties.System.Media.AuthorUrl.Value       = mediaFile.AuthorURL;
                    shellFile.Properties.System.Music.Artist.Value          = mediaFile.ContributingArtists ?? Array.Empty<string>();
                    shellFile.Properties.System.Music.Genre.Value           = mediaFile.Genre ?? Array.Empty<string>();
                    shellFile.Properties.System.Media.Publisher.Value       = mediaFile.Publisher;
                }
            }
            catch (Exception e)
            {
                new ErrorLog().WriteToLog(e.Message, mediaFile.FilePath);
                hasErrors = true;    
            }

            return !hasErrors;
        }
        
        public Dictionary<String, Binding> GenerateBindings <T> (Type MediaType)
        {
            var d = new Dictionary<String, Binding>();

            d.Add(String.Empty, new Binding("isChecked"));
            d.Add("File Name",  new Binding("FileName"));
            d.Add("Title",      new Binding("Title"));
            if (MediaType.Name != "Audio") { d.Add("Tags", new Binding("_Tags")); }
            d.Add("Rating",     new Binding("Rating"));
            d.Add("Comments",   new Binding("Comments"));

            //d.Add("Copyright", new Binding("Copyright")); //Overwrite read-only setting NYI

            if (MediaType.Name == "Picture")
            {
                d.Add("Subject",        new Binding("Subject"));
                d.Add("Author",         new Binding("_Authors"));
                d.Add("Program Name",   new Binding("ProgramName"));
                d.Add("Date Taken",     new Binding("DateTaken"));
                d.Add("Date Acquired",  new Binding("DateAcquired"));
            }
            else if (MediaType.Name == "Audio" || MediaType.Name == "Video")
            {
                //When implemented this is where the "Music" video fields will be added.
                d.Add("Contributing Artists",   new Binding("_ContributingArtists"));
                d.Add("Author URL",             new Binding("AuthorURL"));
                d.Add("Publisher",              new Binding("Publisher"));
                d.Add("Genre",                  new Binding("_Genre"));
                d.Add("Subtitle",               new Binding("Subtitle"));
                
                if (MediaType.Name == "Video")
                {
                    d.Add("Directors",          new Binding("_Directors"));
                    d.Add("Writers",            new Binding("_Writers"));
                    //d.Add("Media Created",      new Binding("MediaCreated"));
                    d.Add("Year",               new Binding("Year"));
                    d.Add("Producers",          new Binding("_Producers"));
                    d.Add("Promotional URL",    new Binding("PromoURL"));
                }
                else
                {
                    d.Add("Composers",      new Binding("_Composers"));
                    d.Add("Album Artist",   new Binding("AlbumArtist"));
                    d.Add("Album",          new Binding("Album"));
                    
                    //We're not implenting this one. Track numbers, much like names are unique and shouldn't be part of the mass editing process. 
                    //However, if single column editing is added later... then maybe.
                    //d.Add("Track Number",   new Binding("TrackNumber")); 
                    //d.Add("BPM",            new Binding("BPM")); //Read above comment.
                }
            }
            return d;
        }
    }
    
    class Picture : Media
    {
        /* Editable extened properties for Pictures

        -----Media-----
        Contributing Artists Album Artist, Album, Year, Genre, Track Number

        ------ Origin ------
         Authors, Date Taken, Program Name, Date Aquired

        ------ Camera/Photo* ------
        
        * Not implemented
        */

        public Picture(){/*Default Constructor*/}

        public Picture(string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath        = filePath;
            Authors         = file.Properties.System.Author.Value;
            Tags            = file.Properties.System.Keywords.Value;

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

        public String[] Authors         { get; set; }
        public String _Authors          { get { return (Authors == null) ? String.Empty : String.Join(";", Authors); }}
        public String ProgramName       { get; set; }
        public DateTime? DateAcquired   { get; set; }
        public DateTime? DateTaken      { get; set; }
        public String Copyright         { get; set; }
    }

    class Audio : Media
    {
        /* Editable extened properties for Audio

        -----Media-----
        Contributing Artists Album Artist, Album, Year, Genre, Track Number

        ------ Origin ------
        Publisher, Author URL

        ------ Content* ------
        Parental Rating, Composers, Conductors, Period
        Part of Set, Inital Key, BPM, Mood, Group Desc.

        * Not all of these are currently not implemented
        */

        public String[] Composers               { get; set; }
        public String _Composers                { get { return (Composers == null) ? String.Empty : String.Join(";", Composers); } }
        public String AlbumArtist               { get; set; }
        public String Album                     { get; set; }
        public uint? TrackNumber                { get; set; }
        public String Copyright                 { get; set; }
        public String Creator                   { get; set; }
        public String BPM                       { get; set; }

        public Audio(string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath = filePath;
            FileName                = file.Properties.System.FileName.Value;

            Composers               = file.Properties.System.Music.Composer.Value;
            ContributingArtists     = file.Properties.System.Music.Artist.Value;
            Genre                   = file.Properties.System.Music.Genre.Value;
            Tags                    = file.Properties.System.Keywords.Value;

            Comments                = file.Properties.System.Comment.Value;
            Copyright               = file.Properties.System.Copyright.Value;
            Rating                  = file.Properties.System.Rating.Value;
            Title                   = file.Properties.System.Title.Value;
            

            AlbumArtist             = file.Properties.System.Music.AlbumArtist.Value;
            Album                   = file.Properties.System.Music.AlbumTitle.Value;
            TrackNumber             = file.Properties.System.Music.TrackNumber.Value;
            BPM                     = file.Properties.System.Music.BeatsPerMinute.Value;

            Publisher               = file.Properties.System.Media.Publisher.Value;
            Subtitle                = (String.IsNullOrEmpty(file.Properties.System.Media.Subtitle.Value)) ? String.Empty : file.Properties.System.Media.Subtitle.Value;
            //Creator                 = file.Properties.System.Media.Creator.Value; WTF WHERE DID THIS GO?!
        }

        public Audio() {/*Default Constructor*/}
    }

    class Video : Media
    {
        /* Editable extened properties for Videos
            
            -----Media-----
            Contributing Artists, Year, Genre

            ------ Origin ------
            Directors, Producers, Writers, Publisher
            Content Provider, Media Created
            Author URL, Promotion URL

            ------ Content* (These Values are set in "Music" Videos.) ------
            Parental Rating, Composers, Conductors, Period, Part of Set, Inital Key, BPM

            * These are currently not implemented
         */

        public uint? Year                   { get; set; }
        public String[] Directors           { get; set; }
        public String[] Writers             { get; set; }
        public String[] Producers           { get; set; }
        public String _Directors            { get { return (Directors == null) ? String.Empty : String.Join(";", Directors); }}
        public String _Writers              { get { return (Writers == null) ? String.Empty : String.Join(";", Writers); }}
        public String _Producers            { get { return (Producers == null) ? String.Empty : String.Join(";", Producers); }}
        //public DateTime? MediaCreated       { get; set; }
        public String PromoURL              { get; set; }


        public Video (string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath = filePath;
            FileName = file.Properties.System.FileName.Value;

            ContributingArtists =   file.Properties.System.Music.Artist.Value;
            Directors =             file.Properties.System.Video.Director.Value;
            Producers =             file.Properties.System.Media.Producer.Value;
            Writers =               file.Properties.System.Media.Writer.Value;
            Genre =                 file.Properties.System.Music.Genre.Value;
            Tags =                  file.Properties.System.Keywords.Value;

            //MediaCreated =          file.Properties.System.DateCreated.Value;

            Comments =  file.Properties.System.Comment.Value;
            Rating =    file.Properties.System.Rating.Value;
            Title =     file.Properties.System.Title.Value;

            AuthorURL = file.Properties.System.Media.AuthorUrl.Value;
            PromoURL =  file.Properties.System.Media.PromotionUrl.Value;

            Publisher = file.Properties.System.Media.Publisher.Value;
            Year =      file.Properties.System.Media.Year.Value;
            Publisher = file.Properties.System.Media.Publisher.Value;
            Subtitle =  file.Properties.System.Media.Subtitle.Value;
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