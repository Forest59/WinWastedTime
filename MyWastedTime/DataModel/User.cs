using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWastedTime.Data
{
  
        [Table("Users")]
        public class User
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString()
            {
                return Name;
            } 

        }
    
}
