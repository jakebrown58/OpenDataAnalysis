/*
 * User: Jake
 * Date: 4/2/2011
 * Time: 9:31 PM
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataElements.DataStore.Interfaces
{

    public interface IIntReader
    {
        IEnumerable<int> Read();
    }

    public interface IReader<T>
	{
        IEnumerable<T> Read();
	}


    public interface IMeasurementReader<T>
    {
        T ReadFromString(string s);
    }


    //public interface IReader<T, X> where T : IEnumerable<X>
    //{
    //    T Read();
    //}
}
