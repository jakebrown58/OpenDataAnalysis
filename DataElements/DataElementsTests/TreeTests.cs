using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataElements.Model.Tree;
using DataElements.Model.Graph;

namespace DataElementsTests
{
    /// <summary>
    /// Summary description for GraphTests
    /// </summary>
    [TestClass]
    public class TreeTests
    {
        #region Junk

        public TreeTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region TestMethods

        [TestMethod]
        public void TestTreeInOrder()
        {
            MyTree t = new MyTree();
            t.AddValue(2);
            t.AddValue(3);
            t.AddValue(1);
            t.AddValue(7);

            List<int> i = t.GetInOrder();
            Assert.AreEqual(4, i.Count);
            Assert.AreEqual(1, i[0]);
            Assert.AreEqual(2, i[1]);
            Assert.AreEqual(3, i[2]);
            Assert.AreEqual(7, i[3]);
        }

        [TestMethod]
        public void TestTreePerf()
        {
            MyTree t = new MyTree();
            Random myR = new Random();
            for (int i = 0; i < 20000; i++)
            {
                t.AddValue(myR.Next(800000));
            }

            TimeSpan timer = TimeTrail(t);
            Console.WriteLine("B-Tree: " + timer.TotalMilliseconds);


            int prev = int.MinValue;
            foreach (int i in t.GetInOrder())
            {
                Assert.IsTrue(prev <= i);
                prev = i;
            }

            MyTree linearTree = new MyTree();
            for (int i = 0; i < 200; i++)
            {
                linearTree.AddValue(i);
            }
        }

        private TimeSpan TimeTrail(I_Tree m)
        {
            DateTime dt0 = DateTime.Now;
            m.GetInOrder();
            DateTime dt1 = DateTime.Now;

            TimeSpan ts0 = new TimeSpan(dt1.Ticks - dt0.Ticks);
            return ts0;
        }


        #endregion

        #region Test Setup

        public class BinaryCode
        {
            public string[] Decode(string message)
            {
                string[] s = new string[2];

                s[0] = decode(message, 0);
                s[1] = decode(message, 1);

                return s;
            }

            private string decode(string message, int firstValue)
            {
                bool isSuccess = false;
                string result = "NONE";
                char[] valuesAsChar = new char[message.Length];
                valuesAsChar[0] = firstValue.ToString()[0];

                for (int i = 0; i < message.Length; i++)
                {
                    char value = 'O';
                    valuesAsChar[i] = (value);
                }

                if (isSuccess)
                {
                    result = valuesAsChar.ToString();
                }

                return result;
            }
        }

        #endregion


        [TestMethod]
        public void Heap_BasicTest()
        {
            BinaryHeap<int> heap = new BinaryHeap<int>();
            heap.Insert(Factory(5));
            heap.Insert(Factory(2));
            heap.Insert(Factory(3));
            heap.Insert(Factory(6));

            AssertPop(heap, 2);
            AssertPop(heap, 3);
            AssertPop(heap, 5);
            heap.Insert(Factory(1));
            AssertPop(heap, 1);
            AssertPop(heap, 6);


        }

        

        private static void AssertPop(BinaryHeap<int> heap, int value)
        {
            int n = heap.DeleteTop();
            Assert.AreEqual(value, n );
        }

        private int Factory(int priority)
        {
            return priority;
        }

    }
}

