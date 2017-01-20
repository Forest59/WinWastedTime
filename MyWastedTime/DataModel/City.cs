using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWastedTime.Data
{
     [Table("City")]
    public class City
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public City()
        { }

        public City(string name)
        {
            Name = name;
        }
        public override string ToString () 
		{ 
 			return Name; 
 		} 

    }

    // train,auto,bus,metro
     [Table("TripKind")]
     public class TripKind
     {
         [PrimaryKey, AutoIncrement]
         public int Id { get; set; }

         public string Name { get; set; }

         public TripKind()
         { }

         public TripKind(string name)
         {
             Name = name;
         }

         public override string ToString()
         {
             return Name;
         } 
     }

    // aller, retour
     [Table("TripType")]
     public class TripType
     {
         [PrimaryKey, AutoIncrement]
         public int Id { get; set; }

         public string Name { get; set; }

         public TripType()
         {
         }

         public TripType(string name)
         {
             Name = name;
         }

         public override string ToString()
         {
             return Name;
         } 
     }
}
