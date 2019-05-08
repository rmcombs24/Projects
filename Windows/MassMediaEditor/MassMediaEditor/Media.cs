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

        #region Properties
        public bool isChecked { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public MediaProperty Title {get; set;}
        public MediaProperty Subtitle { get; set; }
        public MediaProperty Comments { get; set; }
        public MediaProperty Rating { get; set; }
        public MediaProperty Tags { get; set; }
        #endregion

     
    public bool WriteToShellFile(Media mediaFile, bool sort)
    {
        bool hasErrors = false;

        ShellFile shellFile = ShellFile.FromFilePath(mediaFile.FilePath);

        try
        {
            shellFile.Properties.System.FileName.Value  = mediaFile.FileName; //ToDo: Add FileName Editing later, as it might be better for the Append/Prepend program.
            shellFile.Properties.System.Title.Value     = mediaFile.Title.StrValue;
            shellFile.Properties.System.Comment.Value   = mediaFile.Comments.StrValue;
            if (!(mediaFile is Audio)) { shellFile.Properties.System.Keywords.Value = mediaFile.Tags.ArrayValue ?? Array.Empty<string>(); }

            if (mediaFile.Subtitle != null) { shellFile.Properties.System.Media.Subtitle.Value = mediaFile.Subtitle.StrValue; }

            if (mediaFile.Rating.UintValue == 0) { shellFile.Properties.System.Rating.Value = null; }
            else { shellFile.Properties.System.Rating.Value = mediaFile.Rating.UintValue; }

            if (mediaFile is Picture)
            {
                //shellFile.Properties.System.Copyright.Value         = ((Picture)mediaFile).Copyright;
                    
                shellFile.Properties.System.Subject.Value           = ((Picture)mediaFile).Subject.StrValue;
                shellFile.Properties.System.Author.Value            = ((Picture)mediaFile).Authors.ArrayValue ?? Array.Empty<string>();
                shellFile.Properties.System.ApplicationName.Value   = ((Picture)mediaFile).ProgramName.StrValue;
                shellFile.Properties.System.DateAcquired.Value      = (((Picture)mediaFile).DateAcquired.DtValue == DateTime.MinValue) ? null : ((Picture)mediaFile).DateAcquired.DtValue;
                shellFile.Properties.System.Photo.DateTaken.Value   = (((Picture)mediaFile).DateTaken.DtValue == DateTime.MinValue) ? null : ((Picture)mediaFile).DateTaken.DtValue;
            }
            else if (mediaFile is Audio || mediaFile is Video)
            {
                if (mediaFile is Video)
                {
                    shellFile.Properties.System.DateCreated.Value           = (((Video)mediaFile).MediaCreated.DtValue != DateTime.MinValue) ? ((Video)mediaFile).MediaCreated.DtValue : null;
                    shellFile.Properties.System.Media.Writer.Value          = ((Video)mediaFile).Writers.ArrayValue ?? Array.Empty<string>();
                    shellFile.Properties.System.Media.Producer.Value        = ((Video)mediaFile).Producers.ArrayValue ?? Array.Empty<string>();
                    shellFile.Properties.System.Video.Director.Value        = ((Video)mediaFile).Directors.ArrayValue ?? Array.Empty<string>();
                    shellFile.Properties.System.Media.PromotionUrl.Value    = ((Video)mediaFile).PromoURL.StrValue;
                    shellFile.Properties.System.Media.Year.Value            = ((Video)mediaFile).Year.UintValue;
                }
                else if (mediaFile is Audio)
                {
                    shellFile.Properties.System.Music.AlbumArtist.Value     = ((Audio)mediaFile).AlbumArtist.StrValue;
                    shellFile.Properties.System.Music.AlbumTitle.Value      = ((Audio)mediaFile).Album.StrValue;
                    shellFile.Properties.System.Music.TrackNumber.Value     = ((Audio)mediaFile).TrackNumber.UintValue;
                    shellFile.Properties.System.Music.BeatsPerMinute.Value  = ((Audio)mediaFile).BPM.StrValue;
                    shellFile.Properties.System.Music.Composer.Value        = ((Audio)mediaFile).Composers.ArrayValue ?? Array.Empty<string>();
                }

                //I may have to put them into each one again. I don't think I can one off cast this.
                shellFile.Properties.System.Media.AuthorUrl.Value       = ((Audio)mediaFile).AuthorURL.StrValue;
                shellFile.Properties.System.Music.Artist.Value          = ((Audio)mediaFile).ContributingArtists.ArrayValue ?? Array.Empty<string>();
                shellFile.Properties.System.Music.Genre.Value           = ((Audio)mediaFile).Genre.ArrayValue ?? Array.Empty<string>();
                shellFile.Properties.System.Media.Publisher.Value       = ((Audio)mediaFile).Publisher.StrValue;
            }
        }
        catch (Exception e)
        {
            new ErrorLog().WriteToLog(e.Message, mediaFile.FilePath);
            hasErrors = true;    
        }

        return !hasErrors;
        }
        public Dictionary<String, Binding> GenerateBindings (MediaType mediaType)
        {
            var d = new Dictionary<String, Binding>();

            d.Add(String.Empty, new Binding("isChecked"));
            d.Add("File Name",  new Binding("FileName"));
            d.Add("Title",      new Binding("Title.strValue"));
            if (MediaType.Audio == mediaType) { d.Add("Tags", new Binding("Tags.strValue")); }
            d.Add("Rating",     new Binding("Rating.uintValue"));
            d.Add("Comments",   new Binding("Comments.strValue"));

            //d.Add("Copyright", new Binding("Copyright")); //Overwrite read-only setting NYI

            if (MediaType.Pictures == mediaType)
            {
                d.Add("Subject",        new Binding("Subject.strValue"));
                d.Add("Author",         new Binding("_Authors"));
                d.Add("Program Name",   new Binding("ProgramName.strValue"));
                d.Add("Date Taken",     new Binding("DateTaken.dtValue"));
                d.Add("Date Acquired",  new Binding("DateAcquired.strValue"));
            }
            else if (MediaType.Audio == mediaType || MediaType.Video == mediaType)
            {
                //When implemented this is where the "Music" video fields will be added.
                d.Add("Contributing Artists",   new Binding("_ContributingArtists"));
                d.Add("Author URL",             new Binding("AuthorURL"));
                d.Add("Publisher",              new Binding("Publisher"));
                d.Add("Genre",                  new Binding("_Genre"));
                d.Add("Subtitle",               new Binding("Subtitle"));
                
                if (MediaType.Video == mediaType)
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

        #region Extended Properties
        public MediaProperty Authors { get; set; }
        public MediaProperty Subject { get; set; }
        public MediaProperty ProgramName { get; set; }
        public MediaProperty DateAcquired { get; set; }
        public MediaProperty DateTaken { get; set; }
        //public String Copyright         { get; set; }
        #endregion

        public Picture(string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath        = filePath;
            FileName        = file.Properties.System.FileName.Value;

            Comments        = new MediaProperty(file.Properties.System.Comment.Value, MediaSection.Description);
            Title           = new MediaProperty(file.Properties.System.Title.Value, MediaSection.Description);
            Tags            = new MediaProperty(file.Properties.System.Keywords.Value, MediaSection.Description);
            Subject         = new MediaProperty(file.Properties.System.Subject.Value, MediaSection.Description);
            Rating          = new MediaProperty(file.Properties.System.Rating.Value, MediaSection.Description);

            Authors         = new MediaProperty(file.Properties.System.Author.Value, MediaSection.Origin);
            ProgramName     = new MediaProperty(file.Properties.System.ApplicationName.Value, MediaSection.Origin);
            DateAcquired    = new MediaProperty(file.Properties.System.DateAcquired.Value, MediaSection.Origin);
            DateTaken       = new MediaProperty(file.Properties.System.Photo.DateTaken.Value, MediaSection.Origin);
            
            //Copyright       = file.Properties.System.Copyright.Value;
        }
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

        #region Extended Properties
        public MediaProperty Composers              { get; set; }
        public MediaProperty AuthorURL              { get; set; }
        public MediaProperty AlbumArtist            { get; set; }
        public MediaProperty Album                  { get; set; }
        public MediaProperty TrackNumber            { get; set; }
        public MediaProperty Copyright              { get; set; }
        public MediaProperty Creator                { get; set; }
        public MediaProperty BPM                    { get; set; }
        public MediaProperty ContributingArtists    { get; set; }
        public MediaProperty Genre                  { get; set; }
        public MediaProperty Publisher              { get; set; }
        #endregion

        public Audio(string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath = filePath;
            FileName                = file.Properties.System.FileName.Value;

            BPM                     = new MediaProperty(file.Properties.System.Music.BeatsPerMinute.Value, MediaSection.Content);
            Composers               = new MediaProperty(file.Properties.System.Music.Composer.Value, MediaSection.Content);

            AlbumArtist             = new MediaProperty(file.Properties.System.Music.AlbumArtist.Value, MediaSection.Media);
            Album                   = new MediaProperty(file.Properties.System.Music.AlbumTitle.Value, MediaSection.Media);
            TrackNumber             = new MediaProperty(file.Properties.System.Music.TrackNumber.Value, MediaSection.Media);
            ContributingArtists     = new MediaProperty(file.Properties.System.Music.Artist.Value, MediaSection.Media);
            Genre                   = new MediaProperty(file.Properties.System.Music.Genre.Value, MediaSection.Media);

            Comments                = new MediaProperty(file.Properties.System.Comment.Value, MediaSection.Description);
            Rating                  = new MediaProperty(file.Properties.System.Rating.Value, MediaSection.Description);
            Title                   = new MediaProperty(file.Properties.System.Title.Value, MediaSection.Description);
            Subtitle                = new MediaProperty(file.Properties.System.Media.Subtitle.Value, MediaSection.Description);

            Publisher               = new MediaProperty(file.Properties.System.Media.Publisher.Value, MediaSection.Origin);

            //Copyright               = new MediaProperty(file.Properties.System.Copyright.Value;
            //Creator                 = file.Properties.System.Media.Creator.Value; WTF WHERE DID THIS GO?!
        }
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

        #region Extended Properties
        public MediaProperty Year                   { get; set; }
        public MediaProperty Directors              { get; set; }
        public MediaProperty Writers                { get; set; }
        public MediaProperty Producers              { get; set; }
        public MediaProperty PromoURL               { get; set; }
        public MediaProperty AuthorURL              { get; set; }
        public MediaProperty ContributingArtists    { get; set; }
        public MediaProperty Genre                  { get; set; }
        public MediaProperty MediaCreated           { get; set; }
        public MediaProperty Publisher              { get; set; }
        #endregion

        public Video (string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath = filePath;
            FileName = file.Properties.System.FileName.Value;

            Tags                = new MediaProperty(file.Properties.System.Keywords.Value, MediaSection.Description);
            Subtitle            = new MediaProperty(file.Properties.System.Media.Subtitle.Value, MediaSection.Description);
            Comments            = new MediaProperty(file.Properties.System.Comment.Value, MediaSection.Description);
            Rating              = new MediaProperty(file.Properties.System.Rating.Value, MediaSection.Description);
            Title               = new MediaProperty(file.Properties.System.Title.Value, MediaSection.Description);

            Directors           = new MediaProperty(file.Properties.System.Video.Director.Value, MediaSection.Origin);
            Producers           = new MediaProperty(file.Properties.System.Media.Producer.Value, MediaSection.Origin);
            Writers             = new MediaProperty(file.Properties.System.Media.Writer.Value, MediaSection.Origin);
            MediaCreated        = new MediaProperty(file.Properties.System.DateCreated.Value, MediaSection.Origin);
            AuthorURL           = new MediaProperty(file.Properties.System.Media.AuthorUrl.Value, MediaSection.Origin);
            PromoURL            = new MediaProperty(file.Properties.System.Media.PromotionUrl.Value, MediaSection.Origin);
            Publisher           = new MediaProperty(file.Properties.System.Media.Publisher.Value, MediaSection.Origin);

            ContributingArtists = new MediaProperty(file.Properties.System.Music.Artist.Value, MediaSection.Media);
            Genre               = new MediaProperty(file.Properties.System.Music.Genre.Value, MediaSection.Media);
            Year                = new MediaProperty(file.Properties.System.Media.Year.Value, MediaSection.Media);
        }
    }

    class MediaProperty
    {
        public string StrValue { get; set; }
        public string[] ArrayValue { get; set; }
        public uint? UintValue { get; set; }
        public DateTime? DtValue { get; set; }
        public MediaSection MediaSection { get; set; }

        public MediaProperty(string field, MediaSection section)
        {
            StrValue = field;
            MediaSection = section;
        }
        public MediaProperty(string[] field, MediaSection section)
        {
            ArrayValue = field;
            MediaSection = section;
            StrValue = (ArrayValue == null) ? null : String.Join(";", ArrayValue);
        }
        public MediaProperty(uint? field, MediaSection section)
        {
            UintValue = field;
            MediaSection = section;
        }
        public MediaProperty(DateTime? field, MediaSection section)
        {
            DtValue = field;
            MediaSection = section;
        }
    }

    public enum MediaType
    {
        Audio,
        Video,
        Pictures
    }
    public enum MediaSection
    {
        Description,
        Media,
        Origin,
        Content,
        Camera,
        AdvancedPhoto
    }

}