using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWastedTime.Data
{
    [Table("TripUnit")]
    public class TripUnit
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int FromCityId { get; set; }

        [Ignore]
        public string FromCityName { get; set; }
        
        [Indexed]
        public int DestCityId { get; set; }

        [Ignore]
        public string DestCityName { get; set; }

        public DateTime ProgStart { get; set; }

        public DateTime ProgEnd { get; set; }

        public TimeSpan ProgTime { get; set; }

        [Indexed]
        public int TripKindId { get; set; }

        [Ignore]
        public string TripKindName { get; set; }

        public TripUnit()
        { }

        public TripUnit(int fromCityId, int destCityId, int tripKindId,
            DateTime progStart, DateTime progEnd, TimeSpan progTime)
        {
            FromCityId = fromCityId;
            DestCityId = destCityId;
            TripKindId = tripKindId;
            ProgStart = progStart;
            ProgEnd = progEnd;
            if (progTime.Ticks == 0)
                progTime = progEnd - progStart;
            ProgTime = progTime;

        }

    }
}
