using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataElements.Model.Calculations
{

    public class DateValue
    {
        private DateTime _date;
        private decimal _amount;

        public DateTime Date
        {
            get { return _date; }
        }
        public decimal Value
        {
            get { return _amount; }
        }

        public DateValue(DateTime date, decimal value)
        {
            _date = date;
            _amount = value;
        }

         public override string ToString()
        {
            string s = String.Format("D: {0} V: {1}", _date.ToShortDateString(), _amount.ToString("c"));
            return s;
        }

        public override bool Equals(object obj)
        {
            bool equal = false;

            DateValue other = obj as DateValue;
            if (other != null)
            {
                equal = (this.Date == other.Date) && (this.Value == other.Value);
            }

            return equal;
        }

        public override int GetHashCode()
        {
            // http://www.msnewsgroups.net/group/microsoft.public.dotnet.languages.csharp/topic36405.aspx

            int hash = 23;
            hash = hash * 37 + Date.GetHashCode();
            hash = hash * 37 + Value.GetHashCode();

            return hash;
        }
    }

}
