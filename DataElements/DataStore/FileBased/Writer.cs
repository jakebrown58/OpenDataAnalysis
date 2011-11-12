using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataElements.DataStore.FileBased
{
    public interface ICSVOut
    {
        string ConvertToCSV();
    }

    public class LameWriter<T> where T : ICSVOut
    {
        public static void Write(string fileName, IEnumerable<T> list)
        {
            Write(fileName, list, null);
        }

        public static void Write(string fileName, IEnumerable<T> list, string header)
        {
            using (StreamWriter wr = new StreamWriter(fileName))
            {
                if (!string.IsNullOrEmpty(header))
                {
                    wr.WriteLine(header);
                }

                foreach (T item in list)
                {
                    wr.WriteLine(item.ConvertToCSV());
                }
                wr.Close();
            }
        }
    }

    public class LameWriter
    {
        public static void Write(string fileName, List<string> s)
        {
            using (StreamWriter wr = new StreamWriter(fileName))
            {
                foreach (string str in s)
                {
                    wr.WriteLine(str);
                }
                wr.Close();
            }
        }
    }
}
