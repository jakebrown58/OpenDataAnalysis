using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataElements.Model.Analytics
{
    public class Link<T>
    {
        public IEnumerable<T> Map(IEnumerable<T> source, IEnumerable<Link<T>> linkages)
        {
            foreach (T item in source)
            {
                T returnMe = item;
                foreach (Link<T> link in linkages)
                {
                    if (link.From.Equals(item))
                    {
                        returnMe = link.To;
                        break;
                    }
                }

                yield return returnMe;
            }
        }

        public Link(T from, T to)
        {
            this.To = to;
            this.From = from;
        }

        public T To { get; private set; }
        public T From { get; private set; }
    }
}
