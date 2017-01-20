using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWastedTime.Data
{
         [Table("TripVector")]
        public class TripVector
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public string GroupId { get; set; }
             // guid generated

            public int GroupOrder { get; set; }
             // order if items in group

            [Indexed]
            public int UserId { get; set; }

            [Indexed]
            public int TripUnitId { get; set; }

            [Indexed]
            public int TripTypeId { get; set; }

            public DateTime RealStart { get; set; }

            public DateTime RealEnd { get; set; }

            public TimeSpan RealTime { get; set; }

            [Ignore]
            public TripUnit TUn { get; set; }
            [Ignore]
            public TripType TTn { get; set; }

            public TripVector()
            { }

            public TripVector( string groupid, int groupOrder, int userId, int tripUnitId, int tripTypeId, DateTime realStart, DateTime realEnd, TimeSpan realTime)
            {

                GroupId = groupid;
                GroupOrder = groupOrder;
                UserId = userId;
                TripUnitId = tripUnitId;
                TripTypeId = tripTypeId;
                RealStart = realStart;
                RealEnd = realEnd;
                RealTime = realTime;

                if ((realTime == null) || (realTime.Ticks == 0 ))
                {
                    realTime = realEnd - realStart;
                }
            }

        }

}
