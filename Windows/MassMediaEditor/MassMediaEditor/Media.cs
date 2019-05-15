using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Reflection;
using Microsoft.WindowsAPICodePack.Shell;
using System.Linq;

namespace MassMediaEditor
{
    class Media
    {
        
        /* Editable extened properties for Media

            ----- Description -----
            Title, Subject, Rating, Tags, Comments
        */

       #region Properties
        public bool isChecked           { get; set; }
        public string FileName          { get; set; }
        public string FilePath          { get; set; }
        public MediaProperty Title      { get; set; }
        public MediaProperty Subtitle   { get; set; }
        public MediaProperty Comments   { get; set; }
        public UintMediaProperty Rating { get; set; }
        public ArrayMediaProperty Tags  { get; set; }
        #endregion

        public static MediaSection GetMediaSection(Media mediaObj, string propName)
        {
            propName = propName.Replace(" ", String.Empty); //Strip spaces

            var mediaSectionProp = mediaObj.GetType().GetProperty(propName).GetValue(mediaObj, null);
            return ((MediaProperty) mediaSectionProp).MediaSection;
        }

        public static MediaSection GetMediaSection(MediaProperty property)
        {
            return property.MediaSection;
        }

        public static bool WriteToShellFile(Media mediaFile)
        {
            bool hasErrors = false;

            ShellFile shellFile = ShellFile.FromFilePath(mediaFile.FilePath);

            try
            {
                //shellFile.Properties.System.FileName.Value  = mediaFile.FileName; //ToDo: Add FileName Editing later, as it might be better for the Append/Prepend program.
                shellFile.Properties.System.Title.Value     = mediaFile.Title.AsString();
                shellFile.Properties.System.Comment.Value   = mediaFile.Comments.AsString();
                if (!(mediaFile is Audio)) { shellFile.Properties.System.Keywords.Value = mediaFile.Tags.Value ?? Array.Empty<string>(); }

                if (mediaFile.Subtitle != null) { shellFile.Properties.System.Media.Subtitle.Value = mediaFile.Subtitle.AsString(); }

                if (mediaFile.Rating.Value == 0) { shellFile.Properties.System.Rating.Value = null; }
                else { shellFile.Properties.System.Rating.Value = mediaFile.Rating.Value; }

                if (mediaFile is Picture)
                {
                    //shellFile.Properties.System.Copyright.Value         = ((Picture)mediaFile).Copyright;
                    
                    shellFile.Properties.System.Subject.Value           = ((Picture)mediaFile).Subject.AsString();
                    shellFile.Properties.System.Author.Value            = ((Picture)mediaFile).Authors.Value ?? Array.Empty<string>();
                    shellFile.Properties.System.ApplicationName.Value   = ((Picture)mediaFile).ProgramName.AsString();
                    shellFile.Properties.System.DateAcquired.Value      = (((Picture)mediaFile).DateAcquired.Value == DateTime.MinValue) ? null : ((Picture)mediaFile).DateAcquired.Value;
                    shellFile.Properties.System.Photo.DateTaken.Value   = (((Picture)mediaFile).DateTaken.Value == DateTime.MinValue) ? null : ((Picture)mediaFile).DateTaken.Value;
                }
                else if (mediaFile is Video)
                {
                    shellFile.Properties.System.DateCreated.Value           = (((Video)mediaFile).MediaCreated.Value != DateTime.MinValue) ? ((Video)mediaFile).MediaCreated.Value : null;
                    shellFile.Properties.System.Media.Writer.Value          = ((Video)mediaFile).Writers.Value ?? Array.Empty<string>();
                    shellFile.Properties.System.Media.Producer.Value        = ((Video)mediaFile).Producers.Value ?? Array.Empty<string>();
                    shellFile.Properties.System.Video.Director.Value        = ((Video)mediaFile).Directors.Value ?? Array.Empty<string>();
                    shellFile.Properties.System.Media.PromotionUrl.Value    = ((Video)mediaFile).PromotionalURL.AsString();
                    shellFile.Properties.System.Media.Year.Value            = ((Video)mediaFile).Year.Value;
                    shellFile.Properties.System.Media.AuthorUrl.Value       = ((Video)mediaFile).AuthorURL.AsString();
                    shellFile.Properties.System.Music.Artist.Value          = ((Video)mediaFile).ContributingArtists.Value ?? Array.Empty<string>();
                    shellFile.Properties.System.Music.Genre.Value           = ((Video)mediaFile).Genre.Value ?? Array.Empty<string>();
                    shellFile.Properties.System.Media.Publisher.Value       = ((Video)mediaFile).Publisher.AsString();
                }
                else if (mediaFile is Audio)
                {
                    shellFile.Properties.System.Music.AlbumArtist.Value     = ((Audio)mediaFile).AlbumArtist.AsString();
                    shellFile.Properties.System.Music.AlbumTitle.Value      = ((Audio)mediaFile).Album.AsString();
                    shellFile.Properties.System.Music.TrackNumber.Value     = ((Audio)mediaFile).TrackNumber.Value;
                    shellFile.Properties.System.Music.BeatsPerMinute.Value  = ((Audio)mediaFile).BPM.AsString();
                    shellFile.Properties.System.Music.Composer.Value        = ((Audio)mediaFile).Composer.Value ?? Array.Empty<string>();
                    shellFile.Properties.System.Media.AuthorUrl.Value       = ((Audio)mediaFile).AuthorURL.AsString();
                    shellFile.Properties.System.Music.Artist.Value          = ((Audio)mediaFile).ContributingArtists.Value ?? Array.Empty<string>();
                    shellFile.Properties.System.Music.Genre.Value           = ((Audio)mediaFile).Genre.Value ?? Array.Empty<string>();
                    shellFile.Properties.System.Media.Publisher.Value       = ((Audio)mediaFile).Publisher.AsString();
                }            
            }
            catch (Exception e)
            {
                ErrorLog.WriteToLog(e.Message, e.StackTrace, mediaFile.FilePath);
                hasErrors = true;    
            }

            return !hasErrors;
         }

        public static Dictionary<String, Binding> GenerateBindings (MediaType mediaType)
        {
            var d = new Dictionary<String, Binding>();

            d.Add(String.Empty, new Binding("isChecked"));
            d.Add("File Name",  new Binding("FileName"));
            d.Add("Title",      new Binding("Title.Val"));
            if (MediaType.Audio != mediaType) { d.Add("Tags", new Binding("Tags.Val")); }
            d.Add("Rating",     new Binding("Rating.Value"));
            d.Add("Comments",   new Binding("Comments.Val"));

            //d.Add("Copyright", new Binding("Copyright")); //Overwrite read-only setting NYI

            if (MediaType.Pictures == mediaType)
            {
                d.Add("Subject",        new Binding("Subject.Val"));
                d.Add("Authors",        new Binding("Authors.Val"));
                d.Add("Program Name",   new Binding("ProgramName.Val"));
                d.Add("Date Taken",     new Binding("DateTaken.Value"));
                d.Add("Date Acquired",  new Binding("DateAcquired.Value"));
            }
            else if (MediaType.Audio == mediaType || MediaType.Video == mediaType)
            {
                //When implemented this is where the "Music" video fields will be added.
                d.Add("Contributing Artists",   new Binding("ContributingArtists.Val"));
                d.Add("Author URL",             new Binding("AuthorURL.Val"));
                d.Add("Publisher",              new Binding("Publisher.Val"));
                d.Add("Genre",                  new Binding("Genre.Val"));
                d.Add("Subtitle",               new Binding("Subtitle.Val"));
                
                if (MediaType.Video == mediaType)
                {
                    d.Add("Directors",          new Binding("Directors.Val"));
                    d.Add("Writers",            new Binding("Writers.Val"));
                    d.Add("Media Created",      new Binding("MediaCreated.Value"));
                    d.Add("Year",               new Binding("Year.Value"));
                    d.Add("Producers",          new Binding("Producers.Val"));
                    d.Add("Promotional URL",    new Binding("PromotionalURL.Val"));
                }
                else
                {
                    d.Add("Composer",      new Binding("Composer.Val"));
                    d.Add("Album Artist",   new Binding("AlbumArtist.Val"));
                    d.Add("Album",          new Binding("Album.Val"));
                    
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
        public ArrayMediaProperty Authors { get; set; }
        public MediaProperty Subject { get; set; }
        public MediaProperty ProgramName { get; set; }
        public DateTimeMediaProperty DateAcquired { get; set; }
        public DateTimeMediaProperty DateTaken { get; set; }
        //public String Copyright         { get; set; }
        #endregion

        public Picture(string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath        = filePath;
            FileName        = file.Properties.System.FileName.Value;

            Comments        = new MediaProperty(file.Properties.System.Comment.Value, MediaSection.Description);
            Title           = new MediaProperty(file.Properties.System.Title.Value, MediaSection.Description);
            Tags            = new ArrayMediaProperty(file.Properties.System.Keywords.Value, MediaSection.Description);
            Subject         = new MediaProperty(file.Properties.System.Subject.Value, MediaSection.Description);
            Rating          = new UintMediaProperty(file.Properties.System.Rating.Value, MediaSection.Description);

            Authors         = new ArrayMediaProperty(file.Properties.System.Author.Value, MediaSection.Origin);
            ProgramName     = new MediaProperty(file.Properties.System.ApplicationName.Value, MediaSection.Origin);
            DateAcquired    = new DateTimeMediaProperty(file.Properties.System.DateAcquired.Value, MediaSection.Origin);
            DateTaken       = new DateTimeMediaProperty(file.Properties.System.Photo.DateTaken.Value, MediaSection.Origin);
            
            //Copyright       = file.Properties.System.Copyright.Value;
        }

        public Picture()
        {
            FilePath = null;
            FileName = null;

            Comments        = new MediaProperty(String.Empty, MediaSection.Description);
            Title           = new MediaProperty(String.Empty, MediaSection.Description);
            Tags            = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Description);
            Subject         = new MediaProperty(String.Empty, MediaSection.Description);
            Rating          = new UintMediaProperty(null, MediaSection.Description);

            Authors         = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Origin);
            ProgramName     = new MediaProperty(String.Empty, MediaSection.Origin);
            DateAcquired    = new DateTimeMediaProperty(null, MediaSection.Origin);
            DateTaken       = new DateTimeMediaProperty(null, MediaSection.Origin);

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
        public ArrayMediaProperty Composer                  { get; set; }
        public MediaProperty AuthorURL                      { get; set; }
        public MediaProperty AlbumArtist                    { get; set; }
        public MediaProperty Album                          { get; set; }
        public UintMediaProperty TrackNumber                { get; set; }
        public MediaProperty Copyright                      { get; set; }
        public MediaProperty Creator                        { get; set; }
        public MediaProperty BPM                            { get; set; }
        public ArrayMediaProperty ContributingArtists       { get; set; }
        public ArrayMediaProperty Genre                     { get; set; }
        public MediaProperty Publisher                      { get; set; }
        #endregion

        public Audio(string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath = filePath;
            FileName                = file.Properties.System.FileName.Value;

            BPM                     = new MediaProperty(file.Properties.System.Music.BeatsPerMinute.Value, MediaSection.Content);
            Composer                = new ArrayMediaProperty(file.Properties.System.Music.Composer.Value, MediaSection.Content);

            AlbumArtist             = new MediaProperty(file.Properties.System.Music.AlbumArtist.Value, MediaSection.Media);
            Album                   = new MediaProperty(file.Properties.System.Music.AlbumTitle.Value, MediaSection.Media);
            TrackNumber             = new UintMediaProperty(file.Properties.System.Music.TrackNumber.Value, MediaSection.Media);
            ContributingArtists     = new ArrayMediaProperty(file.Properties.System.Music.Artist.Value, MediaSection.Media);
            Genre                   = new ArrayMediaProperty(file.Properties.System.Music.Genre.Value, MediaSection.Media);

            Comments                = new MediaProperty(file.Properties.System.Comment.Value, MediaSection.Description);
            Rating                  = new UintMediaProperty(file.Properties.System.Rating.Value, MediaSection.Description);
            Title                   = new MediaProperty(file.Properties.System.Title.Value, MediaSection.Description);
            Subtitle                = new MediaProperty(file.Properties.System.Media.Subtitle.Value, MediaSection.Description);

            Publisher               = new MediaProperty(file.Properties.System.Media.Publisher.Value, MediaSection.Origin);
            AuthorURL               = new MediaProperty(file.Properties.System.Media.AuthorUrl.Value, MediaSection.Origin);
            //Copyright               = new MediaProperty(file.Properties.System.Copyright.Value;
            //Creator                 = file.Properties.System.Media.Creator.Value; WTF WHERE DID THIS GO?!
        }

        public Audio()
        {
            FilePath = null;
            FileName = null;
            
            BPM                     = new MediaProperty(String.Empty, MediaSection.Content);
            Composer                = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Content);

            AlbumArtist             = new MediaProperty(String.Empty, MediaSection.Media);
            Album                   = new MediaProperty(String.Empty, MediaSection.Media);
            TrackNumber             = new UintMediaProperty(null, MediaSection.Media);
            ContributingArtists     = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Media);
            Genre                   = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Media);

            Comments                = new MediaProperty(String.Empty, MediaSection.Description);
            Rating                  = new UintMediaProperty(null, MediaSection.Description);
            Title                   = new MediaProperty(String.Empty, MediaSection.Description);
            Subtitle                = new MediaProperty(String.Empty, MediaSection.Description);

            Publisher               = new MediaProperty(String.Empty, MediaSection.Origin);
            AuthorURL               = new MediaProperty(String.Empty, MediaSection.Origin);
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
        public UintMediaProperty Year                       { get; set; }
        public ArrayMediaProperty Directors                 { get; set; }
        public ArrayMediaProperty Writers                   { get; set; }
        public ArrayMediaProperty Producers                 { get; set; }
        public MediaProperty PromotionalURL                 { get; set; }
        public MediaProperty AuthorURL                      { get; set; }
        public ArrayMediaProperty ContributingArtists       { get; set; }
        public ArrayMediaProperty Genre                     { get; set; }
        public DateTimeMediaProperty MediaCreated           { get; set; }
        public MediaProperty Publisher                      { get; set; }
        #endregion

        public Video(string filePath)
        {
            ShellFile file = ShellFile.FromFilePath(filePath);

            FilePath = filePath;
            FileName = file.Properties.System.FileName.Value;

            Tags = new ArrayMediaProperty(file.Properties.System.Keywords.Value, MediaSection.Description);
            Subtitle = new MediaProperty(file.Properties.System.Media.Subtitle.Value, MediaSection.Description);
            Comments = new MediaProperty(file.Properties.System.Comment.Value, MediaSection.Description);
            Rating = new UintMediaProperty(file.Properties.System.Rating.Value, MediaSection.Description);
            Title = new MediaProperty(file.Properties.System.Title.Value, MediaSection.Description);

            Directors = new ArrayMediaProperty(file.Properties.System.Video.Director.Value, MediaSection.Origin);
            Producers = new ArrayMediaProperty(file.Properties.System.Media.Producer.Value, MediaSection.Origin);
            Writers = new ArrayMediaProperty(file.Properties.System.Media.Writer.Value, MediaSection.Origin);
            MediaCreated = new DateTimeMediaProperty(file.Properties.System.DateCreated.Value, MediaSection.Origin);
            AuthorURL = new MediaProperty(file.Properties.System.Media.AuthorUrl.Value, MediaSection.Origin);
            PromotionalURL = new MediaProperty(file.Properties.System.Media.PromotionUrl.Value, MediaSection.Origin);
            Publisher = new MediaProperty(file.Properties.System.Media.Publisher.Value, MediaSection.Origin);

            ContributingArtists = new ArrayMediaProperty(file.Properties.System.Music.Artist.Value, MediaSection.Media);
            Genre = new ArrayMediaProperty(file.Properties.System.Music.Genre.Value, MediaSection.Media);
            Year = new UintMediaProperty(file.Properties.System.Media.Year.Value, MediaSection.Media);
        }

        public Video()
        {
            FilePath = null;
            FileName = null;
            
            Tags = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Description);
            Subtitle = new MediaProperty(String.Empty, MediaSection.Description);
            Comments = new MediaProperty(String.Empty, MediaSection.Description);
            Rating = new UintMediaProperty(null, MediaSection.Description);
            Title = new MediaProperty(String.Empty, MediaSection.Description);
            
            Directors = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Origin);
            Producers = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Origin);
            Writers = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Origin);
            MediaCreated = new DateTimeMediaProperty(null, MediaSection.Origin);
            AuthorURL = new MediaProperty(String.Empty, MediaSection.Origin);
            PromotionalURL = new MediaProperty(String.Empty, MediaSection.Origin);
            Publisher = new MediaProperty(String.Empty, MediaSection.Origin);

            ContributingArtists = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Media);
            Genre = new ArrayMediaProperty(Array.Empty<string>(), MediaSection.Media);
            Year = new UintMediaProperty(null, MediaSection.Media);
        }

    }
}