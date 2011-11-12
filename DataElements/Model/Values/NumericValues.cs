using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataElements.DataStore.Interfaces;
using DataElements.DataStore.FileBased;

namespace DataElements.Model.Values
{

    public interface IValue
    {
        FormattableValue Value { get; }
    }

    public abstract class NumericValueWrapper
    {
        protected FormattableValue value;
        public FormattableValue Value { get { return value; } }

        public NumericValueWrapper() { }

        public NumericValueWrapper(FormattableValue value)
        {
            this.value = value;
        }
    }

    public class RawDataValue : NumericValueWrapper, IValue
    {
        public RawDataValue(FormattableValue value)
            : base(value)
        {
        }
    }

    public class RunningBalanceValue : NumericValueWrapper, IValue
    {
        public RunningBalanceValue(FormattableValue value)
            : base(value)
        {
        }
    }

    public class DerivativeValue : NumericValueWrapper, IValue
    {
        public DerivativeValue(SpecializedDateValue<RunningBalanceValue> previous, SpecializedDateValue<RunningBalanceValue> current)
            : base()
        {
            decimal d = (Decimal) new TimeSpan(current.Date.Ticks - previous.Date.Ticks).TotalDays;
            decimal delta = current.Value.Value.NativeValue - previous.Value.Value.NativeValue;

            decimal value = d == 0M ? 0M : delta / d ;

            this.value = new FormattableValue( current.Value.Value.MeasurementType, value);
        }
    }

    public class SpecializedDateValue<T> where T : IValue
    {
        DateTime dt;
        T value;
        public DateTime Date { get { return dt; } }
        public T Value { get { return value; } }

        public SpecializedDateValue(DateTime dt, T value)
        {
            this.dt = dt;
            this.value = value;
        }
    }

    public class Table
    {
        List<ValuesOnDate> rows;

        public Table(List<ValuesOnDate> rows)
        {
            this.rows = rows;
        }

        public List<ValuesOnDate> Rows
        {
            get { return rows; }
        }

        public List<CorrelationResult> GetCorrelations()
        {
            return null;
        }
    }

    public class Correlator
    {
        public static CorrelationResult Correlate(List<SpecializedDateValue<IValue>> values)
        {
            foreach (SpecializedDateValue<IValue> item in values)
            {
            }
            return new CorrelationResult();
        }
    }

    public class CorrelationResult
    {
        public decimal correlationStrength;
        public string correlationType;
    }

    public class TableFactory
    {
        /// <summary>
        /// Builds a Table from the DailyHistory that includes derivative fields for all RunningBalance fields.
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public static Table BuildTable(DailyHistory history)
        {
            List<ValuesOnDate> rows = new List<ValuesOnDate>();
            IEnumerable<TimedMeasurement> thisHist = history.Get();

            foreach (TimedMeasurement tm in thisHist)
            {
                List<IValue> values = GenerateValuesForTimedMeasurement(rows, tm);
                ValuesOnDate datedValues = new ValuesOnDate(tm.MeasurementTime, values);
                rows.Add( datedValues );
            }

            return new Table(rows); 
        }

        private static List<IValue> GenerateValuesForTimedMeasurement(List<ValuesOnDate> rows, TimedMeasurement tm)
        {
            List<IValue> values = new List<IValue>();

            foreach (FormattableValue v in tm.GetAllMeasurements())
            {
                IValue value = null;
                if (v.MeasurementType == MeasurementUnit.CCF)
                    value = new RunningBalanceValue(v);
                else
                    value = new RawDataValue(v);

                values.Add(value);

                if (value is RunningBalanceValue)
                {
                    DerivativeValue dv = GenerateDerivativeValue(rows, tm, values, value);
                    values.Add(dv);
                }
            }
            return values;
        }

        private static DerivativeValue GenerateDerivativeValue(List<ValuesOnDate> rows, TimedMeasurement tm, List<IValue> values, IValue currentValue)
        {
            DerivativeValue dv = null;

            if (rows.Count > 0)
            {
                ValuesOnDate previousRow = rows[rows.Count - 1];
                IValue priorValue = previousRow.Values[values.Count - 1];

                
                SpecializedDateValue<RunningBalanceValue> prev = new SpecializedDateValue<RunningBalanceValue>(
                    previousRow.Date, priorValue as RunningBalanceValue);
                SpecializedDateValue<RunningBalanceValue> current = new SpecializedDateValue<RunningBalanceValue>(
                    tm.MeasurementTime, currentValue as RunningBalanceValue);

                dv = new DerivativeValue(prev, current);
            }
            return dv;
        }
    }


    /// <summary>
    /// Wrapper for a date and a collection of values associated with that date.
    /// </summary>
    public class ValuesOnDate
    {
        DateTime dt;
        public DateTime Date { get { return dt; } }
        private List<IValue> values;
        public List<IValue> Values { get { return values; } }

        public ValuesOnDate(DateTime dt, List<IValue> values)
        {
            this.dt = dt;
            this.values = values;
        }
    }

    public class ValueColumn<T> where T: IValue
    {
        private ICollection<T> values;
        public ICollection<T> Values { get { return values; } }

        public ValueColumn(ICollection<T> values)
        {
            this.values = values;
        }
    }
}
