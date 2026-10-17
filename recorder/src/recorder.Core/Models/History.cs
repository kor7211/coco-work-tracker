using System.Collections.Generic;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Recorder.Models
{
    [Table("histories")]
    public class History : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("type_id")]
        public long TypeId { get; set; }

        [Column("timezone")]
        public string Timezone { get; set; }

        [Column("started")]
        public DateTimeOffset Started { get; set; }

        [Column("uploaded")]
        public DateTimeOffset Uploaded { get; set; }

        [Column("duration_minutes")]
        public int DurationMinutes { get; set; }

        [Column("break_minutes")]
        public int BreakMinutes { get; set; }

        [Column("titles")]
        public Titles? Titles { get; set; }

        [Column("comments")]
        public Comments? Comments { get; set; }

        [Column("images")]
        public List<ImagePath> ImagePaths { get; set; } = [];
    }

    [Table("titles")]
    public class Titles : BaseModel
    {
        [Column("history_id")]
        public long HistoryId { get; set; }

        [Column("en")]
        public string En { get; set; }

        [Column("ja")]
        public string Ja { get; set; }
    }

    [Table("comments")]
    public class Comments : BaseModel
    {
        [Column("history_id")]
        public long HistoryId { get; set; }

        [Column("en")]
        public string En { get; set; }

        [Column("ja")]
        public string Ja { get; set; }
    }

    [Table("images")]
    public class ImagePath : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("history_id")]
        public long HistoryId { get; set; }

        [Column("path")]
        public string Path { get; set; }
    }
}
