using System;
using System.Collections.Generic;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace Recorder.Core.Models
{
    [Table("work_types")]
    public  class WorkType : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("work_type_names")]

        public Names? Names { get; set; }

        [Column("work_type_descriptions")]

        public Names? Descriptions { get; set; }
    }

    [Table("work_type_names")]
    public class Names : BaseModel
    {
        [Column("type_id")]
        public long TypeId { get; set; }

        [Column("en")]
        public string En { get; set; }

        [Column("ja")]
        public string Ja { get; set; }
    }

    [Table("work_type_descriptions")]
    public class Descriptions : BaseModel
    {
        [Column("type_id")]
        public long TypeId { get; set; }

        [Column("en")]
        public string En { get; set; }

        [Column("ja")]
        public string Ja { get; set; }
    }
}
