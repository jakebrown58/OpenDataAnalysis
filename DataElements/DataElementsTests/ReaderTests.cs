using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataElements.DataStore.Interfaces;
using DataElements.DataStore.FileBased;
using DataElements;
using DataElements.DataStore.Database;
using DataElements.Model.Analytics;

namespace DataElementsTests
{

    public class Target : IIntReader
    {
        #region Methods

        List<int> l = new List<int>();

        public IEnumerable<int> Read()
        {
            return l;

        }

        #endregion

        public double GetAverageMatchingMod(int x)
        {
            // like - check total matching the day of a week.
            var numberGroups =
                 from n in Read()
                 group n by n % x into g
                 select new { Remainder = g.Key, Numbers = g };

            double i = 0;
            foreach (var g in numberGroups)
            {
                i += (double)g.Numbers.Average();
            }

            return i;
        }

        public void Add(int i)
        {
            l.Add(i);
        }

        public IEnumerable<int> GetValueBelow(int x)
        {
            Func<int, int, bool> f = (i, j) => i < j;
            return GetValuesByAction(x, f);
        }

        /// <summary>
        /// Returns the collection of items matching f(item, searchValue)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public IEnumerable<int> GetValuesByAction(int searchValue, Func<int, int, bool> f)
        {
            return
                from n in Read()
                where f(n, searchValue)
                //orderby n
                select n;
        }
    }

    [TestClass]
    public class TargetIntReaderTests
    {
        int marker = 6;
        int iterations = 100;



        [TestMethod]
        public void GetValueBelow_Test()
        {
            Target t = new Target();

            for (int i = 1; i < iterations; i++)
            {
                t.Add(i % 6 + 2);
            }

            DateTime start = DateTime.Now;

            List<int> valuesBelowOrEqualToMarker = new List<int>(t.GetValuesByAction(marker, (m, j) => m <= j));

            DateTime end = DateTime.Now;

            //Assert.Fail(new TimeSpan(end.Ticks - start.Ticks).TotalMilliseconds.ToString());


        }

        [TestMethod]
        public void GetValueBelow2_Test()
        {
            Target t = new Target();

            for (int i = 1; i < iterations; i++)
            {
                t.Add(i % 6 + 2);
            }

            DateTime start = DateTime.Now;

            IEnumerable<int> temp = t.Read();
            List<int> valuesBelowOrEqualToMarker = new List<int>();
            foreach (int i in temp)
            {
                if (i <= marker)
                {
                    valuesBelowOrEqualToMarker.Add(i);
                }
            }

            DateTime end = DateTime.Now;

            //Assert.Fail(new TimeSpan(end.Ticks - start.Ticks).TotalMilliseconds.ToString());

        }

        [TestMethod]
        public void GetValueBelow3_Test()
        {
            Target t = new Target();

            for (int i = 1; i < iterations; i++)
            {
                t.Add(i % 6 + 2);
            }

            DateTime start = DateTime.Now;

            IEnumerable<int> temp = t.Read();
            List<int> valuesBelowOrEqualToMarker = new List<int>(temp);
            valuesBelowOrEqualToMarker = valuesBelowOrEqualToMarker.FindAll(x => x < marker);

            DateTime end = DateTime.Now;

            //Assert.Fail(new TimeSpan(end.Ticks - start.Ticks).TotalMilliseconds.ToString());

        }
    }


    [TestClass]
    public class ReaderTests
    {
        [TestMethod]
        public void StupidFileReaderTest()
        {
            FileReader<TimedMeasurement> fileReader = new FileReader<TimedMeasurement>("E:\\backups\\data.csv");
            IReader<TimedMeasurement> tm = ReaderFactory<TimedMeasurement>.GetDerivativeReader(fileReader);

            DailyHistory history = DailyHistory.ConstructFrom(tm.Read());

            int index = 0;
            history.AddFormulaColumn(0, (f, m) => TimedMeasurement.CalculateDeltaOnTime(index, f, m));

            Extract(history);
            Extract2(history);
            List<string> byDayOfWeek = ProduceBucketizedAverages(history, (x) => (x.MeasurementTime.DayOfWeek));
            List<string> byMonth = ProduceBucketizedAverages(history, (x) => (x.MeasurementTime.Month));
            List<string> histogram = ProduceBucketizedAverages(history, (x) => (x.GetByIndex(1).NativeValue));
            Dictionary<decimal, IEnumerable<TimedMeasurement>> histogram2 = ProduceBucketizedAverages2<TimedMeasurement, decimal>(history.Get(), (x) => (x.GetByIndex(1).NativeValue));
            Dictionary<decimal, int> histogram3 = PivotFunctions.ProduceHistogram<TimedMeasurement, decimal>(history.Get(), (x) => Decimal.Round((x.GetByIndex(1).NativeValue), 0));
            Dictionary<DayOfWeek, IEnumerable<TimedMeasurement>> byDayOfWeek2 = ProduceBucketizedAverages2<TimedMeasurement, DayOfWeek>(history.Get(), (x) => (x.MeasurementTime.DayOfWeek));

            Dictionary<DayOfWeek, decimal> days2 = new Dictionary<DayOfWeek, decimal>();
            foreach (DayOfWeek day in byDayOfWeek2.Keys)
            {
                days2.Add(day, byDayOfWeek2[day].Average(x => x.GetByIndex(1).NativeValue));
                days2[day] = Decimal.Round(days2[day], 2);
            }

            Dictionary<DayOfWeek, decimal> byDayOfWeek3 = PivotFunctions.ProducePivotAnalytic<TimedMeasurement, DayOfWeek, decimal>(history.Get(),
                (x) => (x.MeasurementTime.DayOfWeek),
                (z) => ((Math.Round(z.Average(m => m.GetByIndex(1).NativeValue), 2))));

            Dictionary<int, int> histogram4 = PivotFunctions.ProducePivotAnalytic<TimedMeasurement, int, int>(history.Get(),
                (x) => Convert.ToInt32(x.GetByIndex(1).NativeValue),
                (z) => (z.Count()));


            Dictionary<int, IEnumerable<TimedMeasurement>> histogram79 = PivotFunctions.ProducePivotAnalytic<TimedMeasurement, int, IEnumerable<TimedMeasurement>>(history.Get(),
                (x) => Convert.ToInt32(x.GetByIndex(1).NativeValue),
                (z) => (z));

            Dictionary<DayOfWeek, IEnumerable<TimedMeasurement>> histogram83 = PivotFunctions.GeneratePivotData<DayOfWeek, TimedMeasurement>(history.Get(), (x) => x.MeasurementTime.DayOfWeek);

            Dictionary<int, int> hisogram5 = PivotFunctions.GenerateCategoricalCounts<int, TimedMeasurement>(history.Get(), (x) => Convert.ToInt32(x.GetByIndex(1).NativeValue));
            Dictionary<DayOfWeek, int> hisogram6 = PivotFunctions.GenerateCategoricalCounts<DayOfWeek, TimedMeasurement>(history.Get(), (x) => x.MeasurementTime.DayOfWeek);
        }

        private Dictionary<K, IEnumerable<T>> ProduceBucketizedAverages2<T, K>(IEnumerable<T> colletion, Func<T, K> f)
        {
            var grouped =
                from n in colletion
                group n by f(n) into g
                select new { theKey = g.Key, theValue = g };

            Dictionary<K, IEnumerable<T>> dictionary = new Dictionary<K, IEnumerable<T>>();

            foreach (var g in grouped)
            {
                dictionary.Add(g.theKey, null);
                List<T> tList = new List<T>();
                foreach (T item in g.theValue)
                {
                    tList.Add(item);
                }

                dictionary[g.theKey] = tList;
            }

            return dictionary;
        }

        private List<string> ProduceBucketizedAverages(DailyHistory history, Func<TimedMeasurement, object> f)
        {
            var grouped =
                from n in history.Get()
                group n by f(n) into g
                select new { dayOfWeek = g.Key, measurements = g };

            List<string> d = new List<string>();
            foreach (var g in grouped)
            {
                decimal average = g.measurements.Average(m => m.GetByIndex(1).NativeValue);
                int count = g.measurements.Count();

                string dayOfWeek = string.Empty;
                foreach (var n in g.measurements)
                {
                    dayOfWeek = f(n).ToString();
                    break;
                }

                if (dayOfWeek.Length > 10)
                {
                    dayOfWeek = dayOfWeek.Substring(0, 9);
                }

                d.Add(dayOfWeek + ";        Avg:" + average.ToString("n2") + "    Count: " + count);
            }

            return d;
        }

        private void Extract2(DailyHistory history)
        {
            var grouped =
                from n in history.Get()
                //where n.MeasurementTime.Month > 12 || n.MeasurementTime.Month <= 2
                group n by n.MeasurementTime.DayOfWeek into g
                select new { dayOfWeek = g.Key, measurements = g };

            List<string> d = new List<string>();
            foreach (var g in grouped)
            {
                decimal average = g.measurements.Average(m => m.GetByIndex(1).NativeValue);
                string dayOfWeek = string.Empty;
                foreach (var n in g.measurements)
                {
                    dayOfWeek = n.MeasurementTime.DayOfWeek.ToString();
                    break;
                }

                d.Add(dayOfWeek + "; " + average.ToString("n2"));
            }

        }

        private void Extract(DailyHistory history)
        {
            decimal average = history.Get().Average(m => m.GetByIndex(1).NativeValue);

            var sorted =
                from n in history.Get()
                orderby n.GetByIndex(1).NativeValue descending
                select n;

            IEnumerable<TimedMeasurement> numberBelow = sorted.TakeWhile(t => t.GetByIndex(1).NativeValue > 5);

            List<TimedMeasurement> times = new List<TimedMeasurement>();
            foreach (TimedMeasurement measurement in numberBelow)
            {
                times.Add(measurement);
            }
        }
    }
}


