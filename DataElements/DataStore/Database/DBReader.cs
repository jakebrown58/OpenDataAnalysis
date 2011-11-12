using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DataElements.DataStore.Interfaces;
using DatabaseAccessHelper;

namespace DataElements.DataStore.Database
{
    /// <summary>
    /// Reads data from a file on disk.
    /// </summary>
    public class DBReader : IIntReader
    {
        #region private

        StringTable stringTable = new StringTable();

        #endregion

        #region .ctor

        public DBReader()
        {
            DatabaseAccessHelper.DatabaseAccessor dbAccessor = new DatabaseAccessHelper.DatabaseAccessor();
        }

        #endregion

        #region Methods

        public IEnumerable<int> Read()
        {
            return new List<int>();
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

        public IEnumerable<int> GetValueBelow(int x)
        {
            Func<int, int, bool> f = (i, j) => i < j;
            return GetValuesByAction(x, f);
        }

        //public void StoreDailyHistory(DailyHistory dh)
        //{
        //    DatabaseAccessor dbA = new DatabaseAccessor();
        //    foreach (TimedMeasurement tm in dh.Get())
        //    {
        //        dbA.Write("DATED_ENTRY", tm.MeasurementTime, tm.GetByIndex(0).NativeValue, tm.GetByIndex(1).NativeValue);
        //    }
        //}

        /// <summary>
        /// Returns the collection of items matching f(item, searchValue)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public IEnumerable<int> GetValuesByAction(int searchValue, Func<int, int, bool> f)
        {
            var below =
                from n in Read()
                where f(n, searchValue)
                orderby n descending
                select n;

            foreach (var val in below)
                yield return val;
        }
    }

    public interface IDBReaderHelper<T>
    {
        string Get();
        string Store();
        T GetEntity();
    }

    public interface IPersistable
    {
        bool IsStored();

        void GiveNewId(int id);
    }

    public class StringTable
    {
        public List<string> strings = new List<string>();
    }
}

