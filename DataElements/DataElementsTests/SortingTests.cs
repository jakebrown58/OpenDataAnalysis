using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataElementsTests
{
    [TestClass]
    public class SortingTests
    {
        [TestMethod]
        public void BubbleSortTest()
        {
            ISorter<int> s = new BubbleSort();

            bool isInOrder = IsOk(s);

            Assert.IsTrue(isInOrder);
        }

        [TestMethod]
        public void InsertionSortTest()
        {
            ISorter<int> s = new InsertionSort();
            bool isInOrder = IsOk(s);

            Assert.IsTrue(isInOrder);
        }

        [TestMethod]
        public void QuickSortTest()
        {
            ISorter<int> s = new QuickSort();
            bool isInOrder = IsOk(s);

            Assert.IsTrue(isInOrder);
        }

        [TestMethod]
        public void QuickSort_In_Place_Test()
        {
            ISorter<int> s = new QuickSort2();
            bool isInOrder = IsOk(s);

            Assert.IsTrue(isInOrder);
        }


        [TestMethod]
        public void MergeSortTest()
        {
            ISorter<int> s = new MergeSort();
            bool isInOrder = IsOk(s);

            Assert.IsTrue(isInOrder);
        }


        [TestMethod]
        public void TimeTest()
        {
            QuickSort s0 = new QuickSort();
            QuickSort2 s1 = new QuickSort2();
            MergeSort s2 = new MergeSort();

            int max = 100000;

            List<int> list = new List<int>();
            Random r = new Random();
            for (int i = 0; i < max; i++)
            {
                list.Add(r.Next(1000));
            }

            s0.Set(list.ToArray());
            s1.Set(list.ToArray());
            s2.Set(list.ToArray());


            TimeSpan bubbleTime = TimeTrail(s0);
            TimeSpan insertionTime = TimeTrail(s1);
            TimeSpan mergeTime = TimeTrail(s2);


            Console.WriteLine("QuickSort: " + bubbleTime.TotalMilliseconds);
            Console.WriteLine("QuickSort2: " + insertionTime.TotalMilliseconds);
            Console.WriteLine("Merge: " + mergeTime.TotalMilliseconds);

        }

        private TimeSpan TimeTrail(ISorter<int> sorter)
        {

            DateTime dt0 = DateTime.Now;
            sorter.Sort();
            DateTime dt1 = DateTime.Now;

            TimeSpan ts0 = new TimeSpan(dt1.Ticks - dt0.Ticks);
            return ts0;
        }

        [TestMethod]
        public void QuickSort_Concat_Test()
        {
            QuickSort s = new QuickSort();

            int[] left = new int[] { 3, 7, 8 };
            int mid = 1;
            int[] right = new int[] { 2, 4, 8 };
            int[] merged = s.Concat(left, mid, right);

            Assert.IsTrue(merged.Length == 7);
            Assert.IsTrue(merged.Sum() == 33);
        }


        [TestMethod]
        public void MergeSort_Merge_Test()
        {
            MergeSort s = new MergeSort();

            int[] left = new int[] { 3, 7, 8 };
            int[] right = new int[] { 2, 4, 8 };
            int[] merged = s.Merge(left, right);

            Assert.IsTrue(merged.Length == 6);
            Assert.IsTrue(merged.Sum() == 32);
        }

        private static bool IsOk(ISorter<int> s)
        {
            bool before = IsInOrder(s.Get());
            int sumBefore = s.Get().Sum();
            int sumAfter = s.Sort().Sum();
            bool after = IsInOrder(s.Sort());

            return !before && after && sumBefore == sumAfter ;
        }

        private static bool IsInOrder(int[] array)
        {
            bool isInOrder = true;

            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i] > array[i + 1])
                {
                    isInOrder = false;
                }
            }

            return isInOrder;
        }
    }

    public interface ISorter<T>
    {
        T[] Get();
        T[] Sort();
    }

    public abstract class IntSorter
    {
        protected int[] array = new int[] { 5, 7, 8, 1, 4, 3, 9, 2, 6 };

        public int[] Get()
        {
            return array;
        }

        public void Set()
        {
            array = new int[] { 5, 7, 8, 1, 4, 3, 9, 2, 6 };
        }

        public void Set(int[] array)
        {
            this.array = array;
        }
    }

    public class InsertionSort : IntSorter, ISorter<int>
    {
        public int[] Sort()
        {
            int[] sortedArray = array;
            for (int i = 1; i < array.Length; i++)
            {
                int value = sortedArray[i];
                int j = i - 1;
                bool isInPlace = false;

                do
                {
                    if (sortedArray[j] > value)
                    {
                        sortedArray[j + 1] = sortedArray[j];
                        j = j - 1;

                        if (j < 0)
                        {
                            isInPlace = true;
                        }
                    }
                    else
                    {
                        isInPlace = true;
                    }
                } while (!isInPlace);
                sortedArray[j + 1] = value;
            }

            return sortedArray;
        }
    }

    public class BubbleSort : IntSorter, ISorter<int>
    {
        public int[] Sort()
        {
            int[] sortedArray = array;
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = array.Length - 1; j >= i ; j--)
                {
                    int iVal = sortedArray[i];
                    int jVal = sortedArray[j];
                    if (iVal > jVal)
                    {
                        Swap(sortedArray, i, j);
                    }
                }
            }

            return sortedArray;
        }

        private int[] Swap(int[] array, int i, int j)
        {
            int z = array[i];
            array[i] = array[j];
            array[j] = z;
            return array;
        }
    }

    public class MergeSort : IntSorter, ISorter<int>
    {
        public int[] Sort()
        {
            return Sort(array);
        }

        private int[] Sort(int[] array)
        {
            if (array.Length <= 1)
                return array;

            int middle = array.Length / 2;
            int[] left = BuildSubArray(array, 0, middle);
            int[] right = BuildSubArray(array, middle, array.Length);

            left = Sort(left);
            right = Sort(right);

            int[] result = Merge(left, right);
            return result;
        }

        private static int[] BuildSubArray(int[] array, int start, int end)
        {
            int[] subArray = new int[end - start];

            for (int i = start; i < end; i++)
            {
                subArray[i - start] = array[i];
            }
            return subArray;
        }

        public int[] Merge(int[] left, int[] right)
        {
            int[] mergedList = new int[left.Length + right.Length];

            int lLength = left.Length;
            int rLength = right.Length;

            if (lLength == 0)
                mergedList = right;
            else if (rLength == 0)
                mergedList = left;
            else
            {
                int lUsed = 0;
                int rUsed = 0;

                bool lStop = false;
                bool rStop = false;

                for (int i = 0; i < mergedList.Length; i++)
                {
                    bool useLeft = rStop || ( !lStop && left[lUsed] < right[rUsed]);
                    
                    if (useLeft)
                    {
                        mergedList[i] = left[lUsed];
                        lUsed++;
                        if (lUsed == lLength)
                            lStop = true;
                    }
                    else
                    {
                        mergedList[i] = right[rUsed];
                        rUsed++;
                        if (rUsed == rLength)
                            rStop = true;
                    }
                }
            }

            return mergedList;
        }
    }

    public class QuickSort : IntSorter, ISorter<int>
    {
        public int[] Sort()
        {
            return Sort(array);
        }

        private int[] Sort(int[] array)
        {
            if (array.Length <= 1)
                return array;

            int middle = array.Length / 2;
            int pivot = array[array.Length / 2];

            int[] lowerThanPivot = new int[array.Length];
            int[] higherThanPivot = new int[array.Length];

            int lowerIndex = 0;
            int higherIndex = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (i == middle)
                    continue;

                bool lessThanPivot = array[i] < pivot;
                if (lessThanPivot)
                {
                    lowerThanPivot[lowerIndex] = array[i];
                    lowerIndex++;
                }
                else
                {
                    higherThanPivot[higherIndex] = array[i];
                    higherIndex++;
                }
            }

            int[] lower = Sort(BuildSubArray(lowerThanPivot, lowerIndex));
            int[] higher = Sort(BuildSubArray(higherThanPivot, higherIndex));
            int[] result = Concat(lower, pivot, higher);
            return result;
        }

        private static int[] BuildSubArray(int[] array, int end)
        {
            int[] subArray = new int[end];
            for (int i = 0; i < end; i++)
            {
                subArray[i] = array[i];
            }
            return subArray;
        }

        internal int[] Concat(int[] a, int b, int[] c)
        {
            int max = a.Length + 1 + c.Length;
            int[] concatenated = new int[max];
            for (int i = 0; i < a.Length; i++)
            {
                concatenated[i] = a[i];
            }
            concatenated[a.Length] = b;

            for (int i = a.Length + 1; i < max; i++)
            {
                concatenated[i] = c[i - ( a.Length + 1)];
            }

            return concatenated;
        }

        private int[] Sort2(int[] array, int left, int right)
        {
            if (left < right)
            {
                int pivotIndex = (right + left) / 2;
                int newPivot = Partition(array, left, right, pivotIndex);

                Sort2(array, left, newPivot - 1);
                Sort2(array, newPivot + 1, right);
            }

            return array;

        }

        private int Partition(int[] array, int left, int right, int pivotIndex)
        {
            int pivot = array[pivotIndex];
            array = Swap(array, pivot, right);
            int tempIndex = left;

            for (int i = left; i < right; i++)
            {
                if (array[i] < pivot)
                    Swap(array, i, tempIndex);
                tempIndex++;
            }
            Swap(array, tempIndex, right);

            return tempIndex;
        }

        private int[] Swap(int[] array, int i, int j)
        {
            int z = array[i];
            array[i] = array[j];
            array[j] = z;
            return array;
        }
     }

    public class QuickSort2 : IntSorter, ISorter<int>
    {
        public int[] Sort()
        {
            return Sort(array, 0, array.Length - 1);
        }

        private int[] Sort(int[] array, int left, int right)
        {
            if (left < right)
            {
                int pivotIndex = (right + left) / 2;
                int newPivot = Partition(array, left, right, pivotIndex);

                Sort(array, left, newPivot - 1);
                Sort(array, newPivot + 1, right);
            }

            return array;

        }

        private int Partition(int[] array, int left, int right, int pivotIndex)
        {
            int pivot = array[pivotIndex];
            array = Swap(array, pivotIndex, right);
            int tempIndex = left;

            for (int i = left; i < right; i++)
            {
                if (array[i] < pivot)
                {
                    Swap(array, i, tempIndex);
                    tempIndex++;
                }
            }
            Swap(array, tempIndex, right);

            return tempIndex;
        }

        private int[] Swap(int[] array, int i, int j)
        {
            int z = array[i];
            array[i] = array[j];
            array[j] = z;
            return array;
        }
    }
}
