using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWastedTime.Data
{
    [Table("TripLink")]
    public class TripLink
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int TripUnitId { get; set; }

        [Indexed]
        public int TripVectorId { get; set; }


        public TripLink()
        { }

        public TripLink(int unitId, int vectorId)
        {
            TripUnitId = unitId;
            TripVectorId= vectorId;
        }

    }
}
