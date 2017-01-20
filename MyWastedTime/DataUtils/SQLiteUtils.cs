using MyWastedTime.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using System.Linq;
using System.Collections.ObjectModel;

namespace MyWastedTime
{
    public class SQLiteUtils
    {
        #region SQLite utils

        private string _dbName = "myWastedTime.db";

        public SQLiteUtils()
        {
        }

        public string DbName
        {
            get { return _dbName; }
            set { _dbName = value; }
        }

        private async Task<bool> CheckDbAsync(string dbName)
        {
            bool dbExist = true;

            if (String.IsNullOrEmpty(dbName))
                dbName = _dbName;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(dbName);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }

        public async Task InitDb(string dbName)
        {
            if (String.IsNullOrEmpty(dbName)) dbName = _dbName;
            bool dbExist = await CheckDbAsync(dbName);
            if (dbExist)
            {
                await CreateUpdate1(dbName);               
            }
            else
            {
                DbName = dbName;
                await CreateDatabaseAsync(dbName);
                //await GetDataSourceAsync();
                await AddDataAsync();
            }

        }

        private async Task CreateUpdate1(string dbName)
        {
            TripComment x = null;
            Stats y = null;
            string err;
            try
            {
                await CreateDatabaseUpdateAsync(dbName);
                y = await ReadStats(1);
                if (y == null)
                    await AddStatsAsync();

                // read by vectorId = 0 - item insered during update, vector0 can not exist
                x = await ReadTripComment(0);
                if (x == null)
                    await AddTripCommentAsync();
            }
            catch (SQLiteException sqle)
            {
                err = sqle.Result.ToString();
            }
        }

        
        private async Task CreateDatabaseUpdateAsync(string dbName)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
            await conn.CreateTableAsync<TripComment>();
            await conn.CreateTableAsync<Stats>();
            
        }

        private async Task CreateDatabaseAsync(string dbName)
        {            
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
            await conn.CreateTablesAsync<User, City>();
            await conn.CreateTablesAsync<TripKind, TripType, TripUnit, TripVector>();
            await conn.CreateTablesAsync<TripComment, Stats>();                  
        }
     
        private async Task AddDataAsync()
        {
            await AddUsersAsync();
            await AddCityAsync();
            await AddTripKindAsync();
            await AddTripTypeAsync();

            await AddTripUnitAsync();
            await AddTripVectorAsync();
            await AddTripCommentAsync();
            await AddStatsAsync();
        }

        #endregion SQLite utils

        #region SQLite stats

        public async Task<Stats> ReadStats(int cityid)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<Stats>("select * from Stats where Id =" + cityid);
            return existingconact.FirstOrDefault<Stats>();
        }

        public async Task UpdateStats(Stats city)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingCity = await ReadStats(city.Id);
            if (existingCity != null)
            {
                existingCity.Version = city.Version;
                await dbConn.UpdateAsync(existingCity);
            }
            else
            {
                await dbConn.InsertAsync(existingCity);
            }
        }

        #endregion SQLite stats

        #region SQLite_TripVector

        private async Task AddTripVectorAsync()
        {
            var tripTypeList = new List<TripVector>
            {
                new TripVector(Guid.NewGuid().ToString(),0,1,1,1,
                    new DateTime(2015,1,1,6,9,0),
                    new DateTime(2015,1,1,7,13,0),
                    new TimeSpan(0)),
                new TripVector(Guid.NewGuid().ToString(),0,1,2,1,
                    new DateTime(2015,1,1,17,17,0),
                    new DateTime(2015,1,1,18,32,0),
                    new TimeSpan(0)),  
            };

            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            int x = await conn.InsertAllAsync(tripTypeList);
        }

        public TripVector NewTripVectorFromUnit(TripUnit tu, int typeId = 1, int userId = 1)
        {
            TripVector tv = new TripVector(Guid.NewGuid().ToString(), 0, userId, tu.Id, typeId,
                    new DateTime(0), new DateTime(0), new TimeSpan(0));
            return InitTripVectorFromUnit(tv,tu,DateTime.Today);
        }

        public TripVector InitTripVectorFromUnit(TripVector tv, TripUnit tu, DateTime dt)
        {
            tv.RealStart = new DateTime(
                dt.Year , dt.Month , dt.Day,
                tu.ProgStart.Hour ,tu.ProgStart.Minute, 0);
            tv.RealEnd = new DateTime(
                dt.Year, dt.Month, dt.Day,
                tu.ProgEnd.Hour, tu.ProgEnd.Minute, 0);

            tv.RealTime = tv.RealEnd - tv.RealStart;
            
            return tv;
        }

        public async Task<TripVector> ReadLastTripVector()
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripVector>("select * from TripVector " );
            return existingconact.LastOrDefault<TripVector>();
        }

        public async Task<int> ReadTripVectorWithType(int TripTypeId)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripVector>("select * from TripVector where TripTypeId =" + TripTypeId);
            return existingconact.Count();
        }
        public async Task<int> ReadTripVectorWithUnit(int TripUnitId)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripVector>("select * from TripVector where TripUnitId =" + TripUnitId);
            return existingconact.Count();
        }
        public async Task<TripVector> ReadTripVector(int ItemId)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripVector>("select * from TripVector where Id =" + ItemId);
            return existingconact.FirstOrDefault<TripVector>();
        }



        public async Task<ObservableCollection<TripVector>> ReadTripVectors()
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);
            List<TripVector> myCollection = await dbConn.Table<TripVector>().ToListAsync();
            ObservableCollection<TripVector> ContactsList = new ObservableCollection<TripVector>(myCollection);

            return ContactsList;
        }

        public async Task InsertTripVector(TripVector newcontact)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            int x = await dbConn.InsertAsync(newcontact);
        }

        public async Task UpdateTripVector(TripVector city)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingTripType = await ReadTripVector(city.Id);
            if (existingTripType != null)
            {
                existingTripType.GroupId = city.GroupId;
                existingTripType.GroupOrder = city.GroupOrder;
                existingTripType.RealEnd = city.RealEnd ;
                existingTripType.RealStart = city.RealStart;
                existingTripType.RealTime = city.RealTime;
                existingTripType.TripTypeId = city.TripTypeId ;
                existingTripType.TripUnitId = city.TripUnitId ;
                existingTripType.UserId = city.UserId;
                int x = await dbConn.UpdateAsync(existingTripType);
            }
        }

        public async Task DeleteTripVector(int Id)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingTripType = await ReadTripVector(Id);
            if (existingTripType != null)
            {

                int x = await dbConn.DeleteAsync(existingTripType);

            }
        }

        #endregion SQLite_TripVector

        #region SQLite_TripUnit

        private async Task AddTripUnitAsync()
        {
            var tripUnitList = new List<TripUnit>();
            tripUnitList.Add(new TripUnit(1, 2, 1, new DateTime(1, 1, 1, 6, 9, 0), new DateTime(1, 1, 1, 7, 13, 0), new TimeSpan(0)));
            tripUnitList.Add(new TripUnit(2, 1, 2, new DateTime(1, 1, 1,17, 17, 0), new DateTime(1, 1, 1,18, 32, 0), new TimeSpan(0)));               
           
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            int x = await conn.InsertAllAsync(tripUnitList);

        }

        public async Task<int> ReadTripUnitWithKind(int KindId)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripUnit>("select * from TripUnit where TripKindId =" + KindId );
            return existingconact.Count();
        }

        public async Task<int> ReadTripUnitWithCity(int CityId)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripUnit>("select * from TripUnit where FromCityId =" + CityId + " or DestCityId =" + CityId);
            return existingconact.Count();
        }

        public async Task<TripUnit> ReadLastTripUnit()
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripUnit>("select * from TripUnit " );
            return existingconact.LastOrDefault<TripUnit>();
        }
        public async Task<TripUnit> ReadTripUnit(int ItemId)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripUnit>("select * from TripUnit where Id =" + ItemId);
            return existingconact.FirstOrDefault<TripUnit>();
        }


        public async Task<ObservableCollection<TripUnit>> ReadTripUnitsView()
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            string sql = "Select tripunit.FromCityId,tripunit.DestCityId,tripunit.ProgStart,tripunit.ProgEnd,tripunit.TripKindId,"; 
            sql += "fcity.Name as FromCityName,dcity.Name as DestCityName, tripkind.Name as TripKindName ";
            sql += "from tripunit ";
            sql += "left join city as fcity on fcity.Id = tripunit.FromCityId ";
            sql += "left join city as dcity on dcity.Id = tripunit.DestCityId ";
            sql += "left join tripkind on tripkind.id = tripunit.TripKindId";

            var existingconact = await dbConn.QueryAsync<dynamic>(sql);

            ObservableCollection<TripUnit> ContactsList = new ObservableCollection<TripUnit>();
            foreach ( var rc in existingconact)
            {
                TripUnit nt = new TripUnit();
                ContactsList.Add(nt);
            }

            return ContactsList;
        }

        public async Task<ObservableCollection<TripUnit>> ReadTripUnits()
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);
            List<TripUnit> myCollection = await dbConn.Table<TripUnit>().ToListAsync();
            ObservableCollection<TripUnit> ContactsList = new ObservableCollection<TripUnit>(myCollection);

            return ContactsList;
        }


        public async Task InsertTripUnit(TripUnit newcontact)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            int x = await dbConn.InsertAsync(newcontact);
        }

        public async Task UpdateTripUnit(TripUnit city)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingTripType = await ReadTripUnit(city.Id);
            if (existingTripType != null)
            {
                existingTripType.DestCityId = city.DestCityId;
                existingTripType.FromCityId = city.FromCityId;
                existingTripType.ProgEnd = city.ProgEnd;
                existingTripType.ProgStart = city.ProgStart;
                existingTripType.ProgTime = city.ProgTime;
                existingTripType.TripKindId = city.TripKindId;
                int x = await dbConn.UpdateAsync(existingTripType);
            }
        }

        public async Task DeleteTripUnit(int Id)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingTripType = await ReadTripUnit(Id);
            if (existingTripType != null)
            {

                int x = await dbConn.DeleteAsync(existingTripType);

            }
        }

        #endregion SQLite_TripUnit

        #region SQLite_TripLink

        public async Task InsertTripLinks(List<TripLink> tripTypeList )
        {            
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            int x = await conn.InsertAllAsync(tripTypeList);
        }

        public async Task InsertTripLink(TripLink newcontact)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            int x = await dbConn.InsertAsync(newcontact);
        }

        public async Task<ObservableCollection<TripLink>> ReadTripLinks(int tripVectorId)
        {

            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var myCollection = await dbConn.QueryAsync<TripLink>("select * from TripLink where TripVectorId =" + tripVectorId);
            ObservableCollection<TripLink> ContactsList = new ObservableCollection<TripLink>(myCollection);
            return ContactsList;


        }

        #endregion SQLite_TripLink

        #region SQLite_TripType

        private async Task AddTripTypeAsync()
        {
            var tripTypeList = new List<TripType>
            {
                new TripType(){ Name="Trip" },
                new TripType(){ Name="Return"}
            };
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            int x = await conn.InsertAllAsync(tripTypeList);
        }

        public async Task<TripType> ReadLastTripType()
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripType>("select * from TripType ");
            return existingconact.LastOrDefault<TripType>();
        }


        public async Task<TripType> ReadTripType(int TripTypeid)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripType>("select * from TripType where Id =" + TripTypeid);
            return existingconact.FirstOrDefault<TripType>();
        }

        public async Task<ObservableCollection<TripType>> ReadTripTypes()
        {

            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);
            List<TripType> myCollection = await dbConn.Table<TripType>().ToListAsync();
            ObservableCollection<TripType> ContactsList = new ObservableCollection<TripType>(myCollection);
            return ContactsList;


        }

        public async Task UpdateTripType(TripType city)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingTripType = await ReadTripType(city.Id);
            if (existingTripType != null)
            {
                existingTripType.Name = city.Name;
                int x = await dbConn.UpdateAsync(existingTripType);
            }
        }

        public async Task InsertTripType(TripType newcontact)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            int x = await dbConn.InsertAsync(newcontact);
        }

        public async Task DeleteTripType(int Id)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingTripType = await ReadTripType(Id);
            if (existingTripType != null)
            {

               int x = await dbConn.DeleteAsync(existingTripType);

            }
        }

        public async Task DeleteAllTripType()
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            int x = await dbConn.DropTableAsync<TripType>();
            CreateTablesResult y = await dbConn.CreateTableAsync<TripType>();
        } 


        #endregion SQLite_TripType

        #region SQLite_TripKind

        private async Task AddTripKindAsync()
        {
            var tripKindList = new List<TripKind> 
            { 
            new TripKind(){ Name="Train"},
            new TripKind(){ Name="Auto"},
            new TripKind(){ Name="Metro"},
            new TripKind(){ Name="Bus"},
            new TripKind(){ Name="Tram"}
            };
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            int x = await conn.InsertAllAsync(tripKindList);
        }

        public async Task<TripKind> ReadLastTripKind()
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripKind>("select * from TripKind ");
            return existingconact.LastOrDefault<TripKind>();
        }

        public async Task<TripKind> ReadTripKind(int TripKindid)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<TripKind>("select * from TripKind where Id =" + TripKindid);
            return existingconact.FirstOrDefault<TripKind>();
        }

        public async Task<ObservableCollection<TripKind>> ReadTripKinds()
        {

            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);
            List<TripKind> myCollection = await dbConn.Table<TripKind>().ToListAsync();
            ObservableCollection<TripKind> ContactsList = new ObservableCollection<TripKind>(myCollection);
            return ContactsList;


        }

        public async Task UpdateTripKind(TripKind city)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingTripKind = await ReadTripKind(city.Id);
            if (existingTripKind != null)
            {
                existingTripKind.Name = city.Name;
                await dbConn.UpdateAsync(existingTripKind);
            }
        }

        public async Task InsertTripKind(TripKind newcontact)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            await dbConn.InsertAsync(newcontact);
        }

        public async Task DeleteTripKind(int Id)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingTripKind = await ReadTripKind(Id);
            if (existingTripKind != null)
            {

                await dbConn.DeleteAsync(existingTripKind);

            }
        }

        public async Task DeleteAllTripKind()
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            await dbConn.DropTableAsync<TripKind>();
            await dbConn.CreateTableAsync<TripKind>();
        } 

 
        #endregion  SQLite_TripKind

        #region SQLite_City

        //public City ReadCity(int cityid)
        //{
        //    using (var dbConn = new SQLiteConnection(_dbName))
        //    {
        //        var existingconact = dbConn.Query<City>("select * from City where Id =" + cityid).FirstOrDefault();
        //        return existingconact;
        //    }
        //}


        public async Task<City> ReadLastCity()
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<City>("select * from City");
            return existingconact.LastOrDefault<City>();
        }
        public async Task<City> ReadCity(int cityid)
        {
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

            var existingconact = await dbConn.QueryAsync<City>("select * from City where Id =" + cityid);
            return existingconact.FirstOrDefault<City>();
        }

        public async Task<ObservableCollection<City>> ReadCities()
        {
      
            SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);
            List<City> myCollection = await dbConn.Table<City>().ToListAsync();
            ObservableCollection<City> ContactsList = new ObservableCollection<City>(myCollection);
            return ContactsList;

                        
        }

        public async Task UpdateCity(City city)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);    
            var existingCity = await ReadCity(city.Id);
            if (existingCity != null)
            {
                existingCity.Name = city.Name;
                await dbConn.UpdateAsync(existingCity);
            }
        }

        //public void UpdateCity(City city)
        //{
        //    using (var dbConn = new SQLiteConnection(_dbName))
        //    {
        //        var existingconact = dbConn.Query<City>("select * from City where Id =" + city.Id).FirstOrDefault();
        //        if (existingconact != null)
        //        {
        //            existingconact.Name = city.Name;
                   
        //            dbConn.RunInTransaction(() =>
        //            {
        //                dbConn.Update(existingconact);
        //            });
        //        }
        //    }
        //}

        public async Task InsertCity(City newcontact)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);          
            int x = await dbConn.InsertAsync(newcontact);
        }

        //public void Insert(City newcontact)
        //{
        //    using (var dbConn = new SQLiteConnection(_dbName ))
        //    {
        //        dbConn.RunInTransaction(() =>
        //        {
        //            dbConn.Insert(newcontact);
        //        });
        //    }
        //}

        public async Task DeleteCity(int Id)
        {
            var dbConn = new SQLiteAsyncConnection(_dbName);
            var existingCity = await ReadCity(Id);
            if (existingCity != null)
                {
                   
                        await dbConn.DeleteAsync(existingCity);
                  
                }
          }

        //public void DeleteCity(int Id)
        //{
        //    using (var dbConn = new SQLiteConnection(_dbName ))
        //    {
        //        var existingconact = dbConn.Query<City>("select * from City where Id =" + Id).FirstOrDefault();
        //        if (existingconact != null)
        //        {
        //            dbConn.RunInTransaction(() =>
        //            {
        //                dbConn.Delete(existingconact);
        //            });
        //        }
        //    }
        //}

        public void DeleteAllCity()
        {
            using (var dbConn = new SQLiteConnection(_dbName ))
            {
                //dbConn.RunInTransaction(() => 
                //   { 
                dbConn.DropTable<City>();
                dbConn.CreateTable<City>();
                dbConn.Dispose();
                dbConn.Close();
                //}); 
            }
        } 

        private async Task AddStatsAsync()
        {
            var st = new Stats();
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            int x = await conn.InsertAsync(st);
        }

          private async Task AddCityAsync()
        {

            var cityList = new List<City>
            {
                new City(){ Name = "Bruxelles Midi"},
                new City(){ Name = "Bruxelles Central"},
                new City(){ Name = "Mons"},
                new City(){ Name = "Saint-Ghislain"},
                new City(){ Name = "Quievrain"}
            };
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            int x = await conn.InsertAllAsync(cityList);
        }

        #endregion SQLite_City

        #region SQLiteComment

          private async Task AddTripCommentAsync()
          {
              // Create a comment list
              var userList = new List<TripComment>()
            {
                new TripComment(0,"TEST"),          
            };
              // Add rows to the User Table
              SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
              int x = await conn.InsertAllAsync(userList);
          }

          public async Task<TripComment> ReadTripComment(int tripVectorId)
          {
              SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);
              var existingconact = await dbConn.QueryAsync<TripComment>("select * from TripComment where TripVectorId =" + tripVectorId);
              return existingconact.FirstOrDefault<TripComment>();
             
          }

          public async Task<TripComment> ReadTripCommentById(int id)
          {
              SQLiteAsyncConnection dbConn = new SQLiteAsyncConnection(_dbName);

              var existingconact = await dbConn.QueryAsync<TripComment>("select * from TripComment where Id =" + id);
              return existingconact.FirstOrDefault<TripComment>();
          }

          public async Task InsertTripComment(TripComment newcontact)
          {
              var dbConn = new SQLiteAsyncConnection(_dbName);
              int x = await dbConn.InsertAsync(newcontact);
          }

          public async Task DeleteTripCommentById(int Id)
          {
              var dbConn = new SQLiteAsyncConnection(_dbName);
              var existingCity = await ReadTripCommentById(Id);
              if (existingCity != null)
              {

                  await dbConn.DeleteAsync(existingCity);

              }
          }

          public async Task DeleteTripComment(int vectorId)
          {
              var dbConn = new SQLiteAsyncConnection(_dbName);
              var existingCity = await ReadTripComment(vectorId);
              if (existingCity != null)
              {
                  await dbConn.DeleteAsync(existingCity);
              }
          }

          public async Task UpdateTripComment(TripComment city)
          {
              var dbConn = new SQLiteAsyncConnection(_dbName);
              var existingCity = await ReadTripCommentById(city.Id);
              if (existingCity != null)
              {
                  existingCity.FreeText = city.FreeText;
                  await dbConn.UpdateAsync(existingCity);
              }
              else
                  await dbConn.InsertAsync(city);
          }

        #endregion SQLiteComment

        #region SQLite_User

          private async Task AddUsersAsync()
        {
            // Create a users list
            var userList = new List<User>()
            {
                new User(){ Name = "ME"},
                new User(){ Name = "GUEST"}
            };
            // Add rows to the User Table
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            int x = await conn.InsertAllAsync(userList);
        }

        private async Task<List<User>> SearchAllUserAsync()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            var allUsers = await conn.QueryAsync<User>("SELECT * FROM Users");
           
            return allUsers;
        }
        private async Task<List<User>>  SearchUserByNameAsync(string name)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);

            var query = conn.Table<User>().Where(x => x.Name.Contains(name));
            var result = await query.ToListAsync();

            return result;
        }

        private async Task UpdateUserNameAsync(string oldName, string newName)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);

            // Retrieve user
            var user = await conn.Table<User>().Where(x => x.Name == oldName).FirstOrDefaultAsync();
            if (user != null)
            {
                // Modify user
                user.Name = newName;

                // Update record
                await conn.UpdateAsync(user);
            }
        }

        private async Task DeleteUserAsync(string name)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);

            // Retrieve user
            var user = await conn.Table<User>().Where(x => x.Name == name).FirstOrDefaultAsync();
            if (user != null)
            {
                // Delete record
                await conn.DeleteAsync(user);
            }
        }
        private async Task DropTableUserAsync(string name)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);
            await conn.DropTableAsync<User>();
        }
        #endregion SQLite_User

        #region SQLite_sample

        private async Task SearchXXByNameAsync(string name)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(_dbName);

            var query = conn.Table<User>().Where(x => x.Name.Contains("Andy"));
            var result = await query.ToListAsync();
            foreach (var item in result)
            {
                // ...
            }

            var allUsers = await conn.QueryAsync<User>("SELECT * FROM Users");
            foreach (var user in allUsers)
            {
                // ...
            }

            var cityUsers = await conn.QueryAsync<User>(
                "SELECT Name FROM Users WHERE City = ?", new object[] { "Rome, Italy" });
            foreach (var user in cityUsers)
            {
                // ...
            }
        }
        #endregion SQLite_sample
    
    }
}
