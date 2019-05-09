using System;

namespace MassMediaEditor
{
    class MediaProperty
    {
        public string Val { get; set; }
        public MediaSection MediaSection { get; set; }
        public String AsString()
        {
            return Val;
        }

        public MediaProperty(string field, MediaSection section)
        {
            Val = field;
            MediaSection = section;
        }

        public MediaProperty() { }
    }

    class DateTimeMediaProperty : MediaProperty
    {
        public DateTime? Value { get; set; }

        public DateTimeMediaProperty(DateTime? field, MediaSection section)
        {
            Value = field;
            Val = (field.HasValue) ? ((DateTime)field).ToString() : String.Empty;
            MediaSection = section;
        }

        public string DateTimeAsString
        {
            set
            {
                Val = (Value.HasValue) ? Value.ToString() : String.Empty;
            }
        }
    }

    class UintMediaProperty : MediaProperty
    {
        public uint? Value { get; set; }

        public UintMediaProperty(uint? field, MediaSection section)
        {
            Value = field;
            Val = (field.HasValue) ? field.ToString() : String.Empty;
            MediaSection = section;
        }

        public string uintAsString
        {
            set
            {
                Val = (Value.HasValue) ? Value.ToString() : String.Empty;
            }
        }
    }

    class ArrayMediaProperty : MediaProperty
    {
        public string[] Value { get; set; }

        public ArrayMediaProperty(string[] field, MediaSection section)
        {
            Value = field;
            MediaSection = section;
            Val = (Value == null) ? String.Empty : String.Join(";", field);
        }

        public string ArrayAsString
        {
            set
            {
                Val = (Value == null) ? String.Empty : String.Join(";", Value);
            }
        }
    }

}
