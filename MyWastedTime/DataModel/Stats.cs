using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWastedTime.Data
{

    [Table("Stats")]
    public class Stats
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Version { get; set; }
        public override string ToString()
        {
            return Version;
        } 

        public Stats()
        {
            Version = "1.0.0.1";
        }

        public Stats(string version)
        {
            Version = version;
        }
    }
}
