using MyWastedTime.Data;
using MyWastedTime.DataView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWastedTime.DataModel
{
    public class DbDatas
    {
        public ObservableCollection<City> citylist { get; set; }
        public ObservableCollection<TripKind> tripkindlist { get; set; }
        public ObservableCollection<TripType> triptypelist { get; set; }
        public ObservableCollection<TripUnit> tripunitlist { get; set; }
        public IOrderedEnumerable<TripVector> tripvectorlist { get; set; }

        public DashboardView dv { get; set; }

        public DbDatas()
        {
            dv = new DashboardView();
        }

        public async Task<int> DbData_Loading()
        {
            citylist = await App.DbSQLite.ReadCities();
            tripkindlist = await App.DbSQLite.ReadTripKinds();
            triptypelist = await App.DbSQLite.ReadTripTypes();
            tripunitlist = await App.DbSQLite.ReadTripUnits();
            foreach (TripUnit x in tripunitlist)
            {
                x.FromCityName = (await App.DbSQLite.ReadCity(x.FromCityId)).Name;
                x.DestCityName = (await App.DbSQLite.ReadCity(x.DestCityId)).Name;
                x.TripKindName = (await App.DbSQLite.ReadTripKind(x.TripKindId)).Name;
            }        

            TimeSpan ttp = new TimeSpan(0);
            TimeSpan tp = new TimeSpan(0);

            tripvectorlist = (await App.DbSQLite.ReadTripVectors()).OrderByDescending(t => t.RealStart);

            dv.LastTime = (tripvectorlist.First<TripVector>()).RealEnd;
            dv.FirstDate = (tripvectorlist.Last<TripVector>()).RealStart;
            dv.NumRecordedTrips = tripvectorlist.Count<TripVector>();
            dv.NumRecordedDays = 0;

            DateTime dtTemp = DateTime.Today.AddDays(2);

            foreach (TripVector x in tripvectorlist)
            {
                x.TUn = await App.DbSQLite.ReadTripUnit(x.TripUnitId);
                x.TUn.FromCityName = (await App.DbSQLite.ReadCity(x.TUn.FromCityId)).Name;
                x.TUn.DestCityName = (await App.DbSQLite.ReadCity(x.TUn.DestCityId)).Name;
                x.TUn.TripKindName = (await App.DbSQLite.ReadTripKind(x.TUn.TripKindId)).Name;
                x.TTn = await App.DbSQLite.ReadTripType(x.TripTypeId);

                tp += x.RealTime;
                ttp += (x.RealEnd - x.RealStart);
                if (dtTemp.Date != x.RealStart.Date) { dv.NumRecordedDays++; }
                dtTemp = x.RealStart;

            }

            dv.WastedTime = tp.ToString();
            dv.RecordedTime = ttp.ToString();

            return 0;

        }
    }
}
