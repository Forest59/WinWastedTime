using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWastedTime.Data
{
     [Table("TripComment")]
    public class TripComment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int TripVectorId { get; set; }

        public string FreeText { get; set; }

        public TripComment()
        { }

        public TripComment( int vectorId, string freeText = "")
        {
            TripVectorId= vectorId;
            FreeText = freeText;
        }
    }
}
