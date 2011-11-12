using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataElements.DataStore.FileBased;

namespace DataElements
{
    public class DailyHistory
    {
        private List<TimedMeasurement> measurements = new List<TimedMeasurement>();

        public static DailyHistory ConstructFrom( IEnumerable<TimedMeasurement> t)
        {
            DailyHistory dh = new DailyHistory();
            foreach( TimedMeasurement m in t )
                dh.Add( m );
            return dh;
        }

        public void Add(TimedMeasurement t)
        {
            measurements.Add(t);
        }

        public IEnumerable<TimedMeasurement> Get()
        {
            return measurements;
        }

        /// <summary>
        /// Adds a column by executing the passed in function against the current measruement & the one on the preceeding timestamp.
        /// It's good for adding a Delta column.
        /// </summary>
        /// <param name="indexColumn"></param>
        /// <param name="func"></param>
        public void AddFormulaColumn(int indexColumn, Func<TimedMeasurement, TimedMeasurement, FormattableValue> func)
        {
            TimedMeasurement prev = null;
            TimedMeasurement current = null;

            MeasurementUnit currentUnit = measurements[0].GetByIndex(indexColumn ).MeasurementType;
            measurements[0].Add( new FormattableValue( currentUnit, 0 ));

            for( int i = 1; i < measurements.Count; i++ )
            {
                prev = measurements[i - 1];
                current = measurements[i];

                TimedMeasurement t = measurements[i];
                t.Add( func( prev, current ));
            }
        }
    }

    public class TimedMeasurements 
    {
        private List<TimedMeasurement> measurements = new List<TimedMeasurement>();

        public void Add(TimedMeasurement t)
        {
            measurements.Add(t);
        }

        public void AddRange(List<TimedMeasurement> t)
        {
            measurements.AddRange(t);
        }

        public IEnumerable<TimedMeasurement> Get()
        {
            return measurements;
        }

        public IEnumerable<DateTime> GetDates()
        {
            foreach (TimedMeasurement tm in measurements)
            {
                yield return tm.MeasurementTime;
            }
        }

        public IEnumerable<TimedMeasurement> GetByDate(DateTime dt)
        {
            foreach (TimedMeasurement tm in measurements)
            {
                if (tm.MeasurementTime.Equals(dt))
                    yield return tm;
            }
        }

        ///// <summary>
        ///// Generates a full-table of measurements for the 2 passed in measurement collections.
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <returns></returns>
        //public static TimedMeasurements MergeMeasurements(List<TimedMeasurements> measurementsToMerge)
        //{
        //    Dictionary<DateTime, int> dates = new Dictionary<DateTime, int>();

        //    foreach (TimedMeasurements measurementLists in measurementsToMerge)
        //    {
        //        foreach (DateTime dt in measurementLists.GetDates())
        //        {
        //            if (!dates.ContainsKey(dt))
        //                dates.Add(dt, 1);
        //            else
        //                dates[dt]++;
        //        }
        //    }

        //    List<TimedMeasurement> measurements = new List<TimedMeasurement>();
        //    foreach (DateTime dt in dates.Keys)
        //    {
        //        TimedMeasurement mergeMeasure = new TimedMeasurement(dt);

        //        foreach( TimedMeasurements mergeTargetList in measurementsToMerge )
        //        {
        //            TimedMeasurement found = null;
        //            foreach (TimedMeasurement tm in mergeTargetList.GetByDate(dt))
        //            {
        //                found = tm;
        //                break;
        //            }

        //            if (found != null)
        //            {
        //                mergeMeasure.Add(found.GetByIndex(1));
        //            }
        //            else
        //            {
        //                FormattableValue default2 = new FormattableValue(MeasurementUnit.Null, 0);
        //                //mergeMeasure.Add(default2);
        //                mergeMeasure.Add(default2);
        //                //mergeMeasure.Add(default2);
        //            }
        //        }
        //        measurements.Add(mergeMeasure);
        //    }

        //    TimedMeasurements returnValue = new TimedMeasurements();
        //    returnValue.AddRange(measurements);
        //    return returnValue;
        //}
        
    }

    public class TimedMeasurement : ICSVOut
    {
        private DateTime measurementTime;
        private List<FormattableValue> measurements;

        public int Count
        {
            get { return measurements.Count; }
        }

        public TimedMeasurement(DateTime measurementTime)
        {
            this.measurementTime = measurementTime;
        }

        public void Add( FormattableValue v )
        {
            if( measurements == null )
                measurements = new List<FormattableValue>();
            measurements.Add( v );
        }

        public DateTime MeasurementTime
        {
            get { return measurementTime; }
        }
        public FormattableValue GetByIndex(int i)
        {
            return measurements[i];
        }
        public IEnumerable<FormattableValue> GetAllMeasurements()
        {
            return measurements;
        }

        public static FormattableValue CalculateDeltaOnTime(int index, TimedMeasurement x0, TimedMeasurement x1)
        {
            FormattableValue x1value = x1.GetByIndex( index);
            FormattableValue x0value = x0.GetByIndex( index);
            decimal rise = (x1value.NativeValue - x0value.NativeValue);
            decimal run = (decimal)new TimeSpan(x1.MeasurementTime.Ticks - x0.MeasurementTime.Ticks).TotalDays;

            decimal result = run == 0 ? 0 : rise / run;
            return new FormattableValue(x1value.MeasurementType, result);
        }

        public override string ToString()
        {
            string s = String.Format("Dt: {0} V0: {1} V1: {2} ",
                measurementTime.ToShortDateString(), measurements.Count > 0 ? measurements[0].ToString() : string.Empty,
                measurements.Count > 1 ? measurements[1].ToString() : string.Empty);
            return s;
        }


        public string ConvertToCSV()
        {
            string s = MeasurementTime.ToShortDateString();
            foreach (FormattableValue v in measurements)
            {
                if (v.MeasurementType != MeasurementUnit.Null)
                {
                    s += ", " + v.NativeValue;
                }
                else
                {
                    s += ", ";
                }
                
            }
            return s;
        }
    }

    //public class MeasurementType
    //{
    //    private int id;
    //    private string name;
    //    private MeasurementUnit unitType;

    //    public int Id { get { return id; } }
    //    public string Name { get { return name; } }
    //    public MeasurementUnit UnitType { get { return unitType; } }
    //}

    public enum MeasurementUnit
    {
        Null = 0,
        Farenheit = 1,
        CCF = 2,
        Gallon = 3,
        Currency = 4
    }

    public class Currency : FormattableValue
    {
        string currencyCode = "USD";

        public string CurrencyCode
        {
            get { return currencyCode; }
        }

         public Currency(decimal value) : base(MeasurementUnit.Currency, Decimal.Round( value, 2 ) )
         {
         }
    }

    public class FormattableValue
    {
        public FormattableValue(MeasurementUnit unit, decimal value)
        {
            this.value = value;
            this.unit = unit;
        }

        protected MeasurementUnit unit;
        private decimal value;

        public override string ToString()
        {
            return  this.NativeValue.ToString("n2");
        }

        #region Properties

        public MeasurementUnit MeasurementType { get { return unit;}  }

        public decimal NativeValue {get {return value ;} }

        //public FormattableValue ConvertToNewMeasurementType( MeasurementUnit convertToUnit )
        //{
        //    decimal newValue = NativeValue;

        //    if( convertToUnit != this.unit )
        //    {
        //        // Convert.
        //    }

        //    FormattableValue v = new FormattableValue( convertToUnit, newValue );

        //    return  v;
        //}

         #endregion
    }
}
