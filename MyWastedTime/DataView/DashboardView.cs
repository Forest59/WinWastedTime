using System;
using System.Collections;
using System.Collections.Generic;

using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;


namespace MyWastedTime.DataView
{

    public class DashboardView
    {
        private string _recordedTime;

        public string RecordedTime
        {
            get { return _recordedTime; }
            set {

                if (value != _recordedTime)
                {
                    _recordedTime = value;
                    NotifyPropertyChanged();
                }
            
            }
        }

        private string _wastedTime;
        private DateTime firstDate;

        public DateTime FirstDate
        {
            get { return firstDate; }
            set
            {
                if (value != firstDate)
                {
                    firstDate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DateTime lastTime;

        public DateTime LastTime
        {
            get { return lastTime; }
            set
            {
                if (value != lastTime)
                {
                    lastTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int numRecordedTrips;

        public int NumRecordedTrips
        {
            get { return numRecordedTrips; }
            set
            {
                if (value != numRecordedTrips)
                {
                    numRecordedTrips = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int numRecordedDays;

        public int NumRecordedDays
        {
            get { return numRecordedDays; }
            set
            {
                if (value != numRecordedDays)
                {
                    numRecordedDays = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Dictionary<string, TimeSpan> _dT;

        public Dictionary<string, TimeSpan> DT
        {
            get { return _dT; }
            set { _dT = value; }
        }

        public string WastedTime
        {
            get { return _wastedTime; }
            set {
                if (value != _wastedTime)
                {
                    _wastedTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DashboardView()
        {
            DT = new Dictionary<string, TimeSpan>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
