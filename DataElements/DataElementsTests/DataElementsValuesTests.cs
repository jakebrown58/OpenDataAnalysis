using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataElements.DataStore.Interfaces;
using DataElements.DataStore.FileBased;
using DataElements;
using DataElements.DataStore.Database;
using DataElements.Model.Values;

namespace DataElementsTests
{
    [TestClass]
    public class DataElementsValuesTests
    {

        [TestMethod]
        public void TableFactory_Simple_Test()
        {
            FileReader<TimedMeasurement> fileReader = new FileReader<TimedMeasurement>("E:\\backups\\data.csv");
            IReader<TimedMeasurement> tm = ReaderFactory<TimedMeasurement>.GetDerivativeReader(fileReader);

            DailyHistory history = DailyHistory.ConstructFrom(tm.Read());

            Table t = TableFactory.BuildTable(history);
            Assert.IsNotNull(t);

            foreach (IValue i in t.Rows[1].Values)
            {
                Assert.IsNotNull(i);
            }
            
        }

        [TestMethod]
        public void TableFactory_Structure_Test()
        {
            FileReader<TimedMeasurement> fileReader = new FileReader<TimedMeasurement>("E:\\backups\\data.csv");
            IReader<TimedMeasurement> tm = ReaderFactory<TimedMeasurement>.GetDerivativeReader(fileReader);

            DailyHistory history = DailyHistory.ConstructFrom(tm.Read());

            Table t = TableFactory.BuildTable(history);

            List<TimedMeasurement> measurements = new List<TimedMeasurement>();
            
            foreach (TimedMeasurement measure in history.Get())
            {
                measurements.Add(measure);
            }

            int expectedCalculationColumnCount = 0;
            foreach (FormattableValue fv in measurements[0].GetAllMeasurements())
            {
                if (fv.MeasurementType == MeasurementUnit.CCF)
                    expectedCalculationColumnCount++;
            }

            Assert.AreEqual(measurements.Count, t.Rows.Count);
            Assert.AreEqual(measurements[0].Count + expectedCalculationColumnCount, t.Rows[0].Values.Count);


            foreach (ValuesOnDate d in t.Rows)
            {
                Console.Write(d.Date.ToString() + "\t");

                foreach (IValue v in d.Values)
                {
                    string output = string.Empty;
                    if (v != null)
                    {
                        output = Math.Round(v.Value.NativeValue, 2).ToString();
                    }
                    Console.Write(output + "\t");
                }
                Console.Write("\n");
            }

        }

        [TestMethod]
        public void TableFactory_Analysis_Test()
        {
            FileReader<TimedMeasurement> fileReader = new FileReader<TimedMeasurement>("E:\\backups\\data.csv");
            IReader<TimedMeasurement> tm = ReaderFactory<TimedMeasurement>.GetDerivativeReader(fileReader);

            DailyHistory history = DailyHistory.ConstructFrom(tm.Read());

            Table t = TableFactory.BuildTable(history);
            t.GetCorrelations();
        }


    }
}

