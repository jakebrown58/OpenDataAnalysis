/*
 * User: Jake
 * Date: 4/2/2011
 * Time: 9:30 PM
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using DataElements.DataStore.Interfaces;

namespace DataElements.DataStore.FileBased
{
	
	public static class ReaderFactory<T>
	{
		public static IReader<T> GetReader()
		{
			return new FileReader<T>("..\\FirstNames.txt" );
		}

        public static IReader<T> GetDerivativeReader( IReader<T> baseReader )
        {
            IReader<T> factoryReader = null;

            FileReader<TimedMeasurement> reader = baseReader as FileReader<TimedMeasurement>;
            if (reader != null)
            {
                factoryReader = new FileMeasurementReader(new HardCodedFileFormatReader(), reader.FileName) as IReader<T>;
            }

            return factoryReader;
        }
	}

    public class FileMeasurementReader : FileReader<TimedMeasurement>
    {
        #region private

        TimedMeasurementReader measurementReader;

        #endregion

        #region .ctor

        public FileMeasurementReader(TimedMeasurementReader measurementReader, string fileName) : base( fileName )
        {
            this.measurementReader = measurementReader;
        }

        #endregion

        #region Methods

        public override IEnumerable<TimedMeasurement> Read()
        {
            return ReadDailyHistory();
        }

        private IEnumerable<TimedMeasurement> ReadDailyHistory()
        {
            base.OpenStreamReader();

            ICollection<TimedMeasurement> history = new List<TimedMeasurement>();

            bool header = true;

            foreach (string s in base.stringTable)
            {
                if (!header)
                {
                    TimedMeasurement measurement = measurementReader.ReadFromString(s);
                    history.Add(measurement);
                }
                header = false;
            }

            return history;
        
        }

        #endregion
    }

	/// <summary>
	/// Reads data from a file on disk.
	/// </summary>
    public class FileReader<T> : IReader<T>
	{
		#region private
		
		protected string fileName = string.Empty;
		protected StringTable stringTable = new StringTable();
		
		#endregion
		
		#region .ctor

        protected FileReader()
        {
        }

		public FileReader( string fileName )
		{
			this.fileName = fileName;
		}
		
		#endregion

        #region Methods

        public virtual IEnumerable<T> Read()
        {
            //OpenStreamReader();
            return new List<T>();
        }

        #endregion

        #region Subroutines

        protected void OpenStreamReader()
        {
            StreamReader re = File.OpenText( fileName );
            string input = null;
            while ( ( input = re.ReadLine() ) != null )
            {
                stringTable.Add( input );
            }

            re.Close();
        }

        #endregion

        #region Properties

        public string FileName
        {
            get { return this.fileName; } 
        }

        #endregion
    }

    public class StringTable : List<string>
	{
	}

    public abstract class TimedMeasurementReader : IMeasurementReader<TimedMeasurement>
    {
        public virtual TimedMeasurement ReadFromString(string s)
        {
            return null;
        }
    }

    public class HardCodedFileFormatReader : TimedMeasurementReader
    {
        public override TimedMeasurement ReadFromString(string s)
        {
            string[] values = s.Split('\t');

            DateTime dt = Convert.ToDateTime(values[0]);
            decimal ccf = Convert.ToDecimal(values[1]);

            FormattableValue v = new FormattableValue(MeasurementUnit.CCF, ccf);

            TimedMeasurement timeMeasurement = new TimedMeasurement(dt);
            timeMeasurement.Add(v);

            return timeMeasurement;
        }
    }
}
