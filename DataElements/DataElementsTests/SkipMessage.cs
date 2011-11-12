using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataElementsTests
{
    [TestClass]
    public class SkipMessage
    {
        [TestMethod]
        public void SkipMessageBasic1Test()
        {
            SkipParams sp = GetDefaultSkipParams();
            ISkipMessager s = new BasicSkipMessager(sp);

            IEnumerable<string> result = s.Get();
            IsOk(result);
        }

        private void IsOk(IEnumerable<string> result)
        {
            List<string> values = Listize(result);

            Assert.AreEqual(values[0], "SeHl");
            Assert.AreEqual(values[1], "lpoo");
        }

        [TestMethod]
        public void SkipMessageBasic4Test()
        {
            SkipParams sp = GetDefaultSkipParams(4);
            ISkipMessager s = new BasicSkipMessager(sp);

            IEnumerable<string> result = s.Get();
            IsOk4(result);
        }

        private void IsOk4(IEnumerable<string> result)
        {
            List<string> values = Listize(result);

            Assert.AreEqual(values[0], "Syo");
            Assert.AreEqual(values[1], "lHw");
            Assert.AreEqual(values[2], "eo");
            Assert.AreEqual(values[3], "el");
            Assert.AreEqual(values[4], "pl");
            Assert.AreEqual(values[5], "yo");
            Assert.AreEqual(values[6], "Hw");
            Assert.AreEqual(values[7], "Hw");
            Assert.AreEqual(values[8], "o");
            Assert.AreEqual(values[9], "l");
            Assert.AreEqual(values[10], "l");
            Assert.AreEqual(values[11], "o");
            Assert.AreEqual(values[12], "w");
        }


        [TestMethod]
        public void SkipMessageOptimized1Test()
        {
            SkipParams sp = GetDefaultSkipParams();
            ISkipMessager s = new LoopReducerSkipMessager(sp);

            IEnumerable<string> result = s.Get();
            IsOk(result);
        }

        [TestMethod]
        public void SkipMessageOptimized4Test()
        {
            SkipParams sp = GetDefaultSkipParams(4);
            ISkipMessager s = new LoopReducerSkipMessager(sp);

            IEnumerable<string> result = s.Get();
            IsOk4(result);
        }

        [TestMethod]
        public void SkipMessageOptimized_LenTest()
        {
            SkipParams sp = BigSkipParams();
            ISkipMessager s1 = new LoopReducerSkipMessager(sp);

            TimeSpan optimizedTime = TimeTrail(s1);

            Console.WriteLine("Skip Redux: " + optimizedTime.TotalMilliseconds);
        }

        [TestMethod]
        public void SkipMessageEquivelencyTest()
        {
            SkipParams sp = BigSkipParams();
            ISkipMessager s0 = new BasicSkipMessager(sp);
            ISkipMessager s1 = new LoopReducerSkipMessager(sp);

            List<string> st0 = Listize(s0.Get());
            List<string> st1 = Listize(s1.Get());

            for( int i = 0; i < st0.Count; i++ )
            {
                Assert.AreEqual(st0[i], st1[i], i.ToString());
            }
        }


        [TestMethod]
        public void SkipMessageTimeTest()
        {
            SkipParams sp = BigSkipParams();
            ISkipMessager s0 = new BasicSkipMessager(sp);
            ISkipMessager s1 = new LoopReducerSkipMessager(sp);

            TimeSpan basicTime = TimeTrail(s0);
            TimeSpan optimizedTime = TimeTrail(s1);

            Console.WriteLine("Basic: " + basicTime.TotalMilliseconds);
            Console.WriteLine("Skip Redux: " + optimizedTime.TotalMilliseconds);
        }

        private TimeSpan TimeTrail(ISkipMessager m)
        {
            DateTime dt0 = DateTime.Now;
            m.Get();
            DateTime dt1 = DateTime.Now;

            TimeSpan ts0 = new TimeSpan(dt1.Ticks - dt0.Ticks);
            return ts0;
        }

        [TestMethod]
        public void MergeFirstTest()
        {
            List<KeyedItem> a = new List<KeyedItem>();
            a.Add( KeyedItem.GetItem( 1, 20 ) );
            a.Add( KeyedItem.GetItem( 2, 25 ) );

            List<KeyedItem> b = new List<KeyedItem>();
            b.Add( KeyedItem.GetItem( 1, 2 ) );
            b.Add( KeyedItem.GetItem( 3, 84 ) );

            TestMerge( new Merger1(), a, b );
            
        }

        [TestMethod]
        public void MergeSecondTest()
        {
            List<KeyedItem> a = new List<KeyedItem>();
            a.Add(KeyedItem.GetItem(1, 20));
            a.Add(KeyedItem.GetItem(2, 25));

            List<KeyedItem> b = new List<KeyedItem>();
            b.Add(KeyedItem.GetItem(1, 2));
            b.Add(KeyedItem.GetItem(3, 84));

            TestMerge(new Merger2(), a, b);
        }

        [TestMethod]
        public void MergeThirdTest()
        {
            List<KeyedItem> a = new List<KeyedItem>();
            a.Add(KeyedItem.GetItem(1, 20));
            a.Add(KeyedItem.GetItem(2, 25));

            List<KeyedItem> b = new List<KeyedItem>();
            b.Add(KeyedItem.GetItem(1, 2));
            b.Add(KeyedItem.GetItem(3, 84));

            TestMerge(new Merger3(), a, b);
        }

        [TestMethod]
        public void MergeFourthTest()
        {
            List<KeyedItem> a = new List<KeyedItem>();
            a.Add(KeyedItem.GetItem(1, 20));
            a.Add(KeyedItem.GetItem(2, 25));

            List<KeyedItem> b = new List<KeyedItem>();
            b.Add(KeyedItem.GetItem(1, 2));
            b.Add(KeyedItem.GetItem(3, 84));

            TestMerge(new Merger4(), a, b);
        }


        private void TestMerge(IMerger merger, List<KeyedItem> a, List<KeyedItem> b)
        {
            List<KeyedItem> m = merger.Merge(a, b);

            Assert.AreEqual(m.Count, 3);
            Assert.AreEqual(m[0].value, 2);
            Assert.AreEqual(m[1].value, 25);
            Assert.AreEqual(m[2].value, 84);

            Assert.AreEqual(m[0].key, 1);
            Assert.AreEqual(m[1].key, 2);
            Assert.AreEqual(m[2].key, 3);
        }

        private List<string> Listize(IEnumerable<string> result)
        {
            List<string> list = new List<string>();
            foreach (string item in result)
            {
                list.Add(item);
            }

            return list;
        }

        public SkipParams BigSkipParams()
        {
            string s = string.Empty;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 50; i++)
            {
                sb.Append("The quick brown fox jumps over the lazy dog.");
            }

            return new SkipParams(sb.ToString(), 4);
        }

        public SkipParams GetDefaultSkipParams()
        {
            return GetDefaultSkipParams(2);
        }

        public SkipParams GetDefaultSkipParams(int offset)
        {
            return new SkipParams("Sleepy Hollow", offset);
        }
    }

    public class SkipParams
    {
        string message;
        int mod;

        public SkipParams(string messsage, int mod)
        {
            this.message = messsage;
            this.mod = mod;
        }

        public string Message
        {
            get { return message; }
        }
        public int Mod
        {
            get { return mod; }
        }

    }

    public interface ISkipMessager
    {
        IEnumerable<string> Get();
    }

    public class BasicSkipMessager : ISkipMessager
    {
        SkipParams skipParams = null;

        public BasicSkipMessager(SkipParams skipParams)
        {
            this.skipParams = skipParams;
        }

        public IEnumerable<string> Get()
        {
            List<string> s = new List<string>();

            for (int i = 0; i < skipParams.Message.Length; i++)
            {
                s.Add(GetSingleMessage(skipParams, i));
            }

            return s;
        }

        private string GetSingleMessage(SkipParams skipParams, int startingLocation)
        {
            char[] result = new char[skipParams.Message.Length];

            int charsAdded = 0;
            int whiteSpaceCount = 0;
            int mod = skipParams.Mod + 1;

            for (int i = startingLocation; i < skipParams.Message.Length; i++)
            {
                if (char.IsWhiteSpace(skipParams.Message[i]))
                {
                    whiteSpaceCount++;
                    continue;
                }

                int iMod = i % mod;
                int modTarget = (whiteSpaceCount + startingLocation) % mod;

                bool addThis = mod == 1 ? true : iMod == modTarget;
                if (addThis)
                {
                    result[charsAdded] = skipParams.Message[i];
                    charsAdded++;
                }
            }

            string resultingValue = string.Empty;
            if (charsAdded > 0)
            {
                resultingValue = new string(result);
                resultingValue = resultingValue.Substring(0, charsAdded);
            }

            return resultingValue;
        }
    }

    public class LoopReducerSkipMessager : ISkipMessager
    {
        SkipParams skipParams = null;

        public LoopReducerSkipMessager(SkipParams skipParams)
        {
            this.skipParams = skipParams;
        }

        public IEnumerable<string> Get()
        {
            List<string> s = new List<string>();

            int length = skipParams.Message.Length;
            int mod = skipParams.Mod + 1;

            Dictionary<int, string> distinctValues = new Dictionary<int, string>();

            int wsC = 0;
            for (int i = 0; i < mod; i++)
            {
                if (char.IsWhiteSpace(skipParams.Message[i]))
                {
                    wsC++;
                }
                distinctValues.Add(i, GetSingleMessage(skipParams, i + wsC, mod, length));
            }

            wsC = 0;
            for (int i = 0; i < length; i++)
            {
                int startingIndex = (i + wsC) / mod;
                int dKey = (i + wsC) % mod;
                int len = distinctValues[dKey].Length;

                if (startingIndex < len)
                {
                    s.Add(distinctValues[dKey].Substring(startingIndex));
                }
                else
                {
                    s.Add(string.Empty);
                }

                if (char.IsWhiteSpace(skipParams.Message[i]))
                {
                    wsC--;
                }
            }

            return s;
        }

        private string GetSingleMessage(SkipParams skipParams, int startingLocation, int mod, int length)
        {
            char[] result = new char[(length / mod) + 1 ];

            int charsAdded = 0;
            int whiteSpaceCount = 0;

            if (startingLocation >= length)
                return string.Empty;

            for (int i = startingLocation; i < skipParams.Message.Length; i++)
            {
                if (char.IsWhiteSpace(skipParams.Message[i]))
                {
                    whiteSpaceCount++;
                    continue;
                }

                int iMod = i % mod;
                int modTarget = (whiteSpaceCount + startingLocation) % mod;
                bool addThis = iMod == modTarget;

                if (addThis)
                {
                    result[charsAdded] = skipParams.Message[i];
                    charsAdded++;
                }
            }

            string resultingValue = string.Empty;
            if (charsAdded > 0)
            {
                resultingValue = new string(result);
                resultingValue = resultingValue.Substring(0, charsAdded);
            }

            return resultingValue;
        }
    }


    public class KeyedItem
    {
        public int key;
        public float value;

        public static KeyedItem GetItem(int key, float value)
        {
            KeyedItem ki = new KeyedItem();
            ki.key = key;
            ki.value = value;
            return ki;
        }
    }

    public interface IMerger
    {
         List<KeyedItem> Merge(List<KeyedItem> a, List<KeyedItem> b);
    }

    public class Merger1 : IMerger
    {
        public List<KeyedItem> Merge(List<KeyedItem> a, List<KeyedItem> b)
        {
            List<KeyedItem> mergeList = new List<KeyedItem>();
            int toProcess = a.Count + b.Count;

            Dictionary<int, int> countByKey = new Dictionary<int, int>();

            int aIndex = 0;
            int bIndex = 0;

            while (toProcess > 0)
            {
                KeyedItem tipOfA = null;
                KeyedItem tipOfB = null;
                if (aIndex < a.Count)
                {
                    tipOfA = a[aIndex];
                }
                if (bIndex < b.Count)
                {
                    tipOfB = b[bIndex];
                }

                bool aOk = tipOfA != null;

                #region Detect collisions and adjust

                if (aOk && countByKey.ContainsKey(tipOfA.key))
                {
                    aOk = false;
                    countByKey[tipOfA.key]++;
                    toProcess--;
                    aIndex++;
                    continue;
                }

                bool bOk = tipOfB != null;
                if (bOk && countByKey.ContainsKey(tipOfB.key))
                {
                    bOk = false;
                    countByKey[tipOfB.key]++;
                    toProcess--;
                    bIndex++;
                    continue;
                }

                #endregion

                bool addA = aOk;
                bool addB = bOk;

                #region figure out which to add

                if (aOk && bOk)
                {
                    if (tipOfA.value < tipOfB.value){
                        bOk = false;
                    }
                    else if (tipOfA.value > tipOfB.value){
                        aOk = false;
                    }
                    // okay to add both, if the values are the same, and the keys are different.
                    // otherwise, discard the one in the 'b' list {by arbitrary convention}.
                    if (tipOfA.key == tipOfB.key && (aOk && bOk))
                    {
                        bOk = false;
                        bIndex++;
                        toProcess--;
                        continue;
                    }

                    addA = aOk;
                    addB = bOk;
                }


                #endregion

                if (addA)
                {
                    countByKey.Add(tipOfA.key, 1);
                    mergeList.Add(tipOfA);
                    toProcess--;
                    aIndex++;
                }
                if (addB)
                {
                    countByKey.Add(tipOfB.key, 1);
                    mergeList.Add(tipOfB);
                    toProcess--;
                    bIndex++;
                }
            }

            return mergeList;
        }
    }

    public class Merger2 : IMerger
    {
        public List<KeyedItem> Merge(List<KeyedItem> a, List<KeyedItem> b)
        {
            List<KeyedItem> mergeList = new List<KeyedItem>();
            int toProcess = a.Count + b.Count;

            Dictionary<int, int> countByKey = new Dictionary<int, int>();

            int aIndex = 0;
            int bIndex = 0;

            while (toProcess > 0)
            {
                KeyedItem tipOfA = null;
                KeyedItem tipOfB = null;
                if (aIndex < a.Count)
                {
                    tipOfA = a[aIndex];
                }
                if (bIndex < b.Count)
                {
                    tipOfB = b[bIndex];
                }
                bool addA = tipOfA != null;
                bool addB = tipOfB != null;

                KeyedItem itemToAdd = null;
                if (!addA)
                {
                    itemToAdd = tipOfB;
                }
                else if (!addB)
                {
                    itemToAdd = tipOfA;
                }
                else
                {
                    addA = tipOfA.value > tipOfB.value ? false : true;
                    addB = !addA;
                    itemToAdd = tipOfA.value > tipOfB.value ? tipOfB : tipOfA;
                }

                if (addA)
                    aIndex++;
                else
                    bIndex++;
                
                if (!countByKey.ContainsKey(itemToAdd.key))
                {
                    countByKey.Add(itemToAdd.key, 1);
                    mergeList.Add(itemToAdd);
                }

                toProcess--;
            }

            return mergeList;
        }
    }

    public class Merger3 : IMerger
    {
        public List<KeyedItem> Merge(List<KeyedItem> a, List<KeyedItem> b)
        {
            List<KeyedItem> mergeList = new List<KeyedItem>();
            int toProcess = a.Count + b.Count;

            Dictionary<int, int> countByKey = new Dictionary<int, int>();

            int aIndex = 0;
            int bIndex = 0;

            while (toProcess > 0)
            {
                bool checkA = aIndex < a.Count;
                bool checkB = bIndex < b.Count; 

                KeyedItem itemToAdd = null;
                if (!checkA)
                {
                    itemToAdd = b[bIndex];
                    bIndex++;
                }
                else if (!checkB)
                {
                    itemToAdd = a[aIndex];
                    aIndex++;
                }
                else
                {
                    bool addA = a[aIndex].value > b[bIndex].value ? false : true;
                    itemToAdd = addA ? a[aIndex] : b[bIndex];
                    if (addA)
                        aIndex++;
                    else
                        bIndex++;
                }

                if (!countByKey.ContainsKey(itemToAdd.key))
                {
                    countByKey.Add(itemToAdd.key, 1);
                    mergeList.Add(itemToAdd);
                }

                toProcess--;
            }

            return mergeList;
        }
    }

    public class Merger4 : IMerger
    {
        public List<KeyedItem> Merge(List<KeyedItem> a, List<KeyedItem> b)
        {
            List<KeyedItem> nonDuplicateBs = RemoveKeys(b, PopulateFromList(a));
            return SimpleMerge( a, nonDuplicateBs );
        }

        private Dictionary<int, KeyedItem> PopulateFromList( List<KeyedItem> a )
        {
            Dictionary<int, KeyedItem > d = new Dictionary<int, KeyedItem>();
            foreach( KeyedItem k in a )
            {
                d.Add(k.key, k);
            }
            return d;
        }

        private List<KeyedItem> RemoveKeys(List<KeyedItem> b, Dictionary<int, KeyedItem> removalKeys)
        {
            List<KeyedItem> subList = new List<KeyedItem>();

            foreach (KeyedItem k in b)
            {
                if (!removalKeys.ContainsKey(k.key))
                    subList.Add(k);
            }

            return subList;
        }

        private List<KeyedItem> SimpleMerge(List<KeyedItem> a, List<KeyedItem> b)
        {
            List<KeyedItem> merge = new List<KeyedItem>();
            int aPos = 0;
            int bPos = 0;

            for (int i = 0; i < a.Count + b.Count; i++)
            {
                bool useLeft = UseLeft(a, b, aPos, bPos);

                if (useLeft)
                {
                    merge.Add(a[aPos]);
                    aPos++;
                }
                else
                {
                    merge.Add(b[bPos]);
                    bPos++;
                }
            }

            return merge;
        }

        private bool UseLeft(List<KeyedItem> a, List<KeyedItem> b, int aPos, int bPos)
        {
            int use = 1;

            if( aPos >= a.Count ) {
                use = 2;
            }
            else if (bPos >= b.Count)
            {
                use = 0;
            }
            else
            {
                use = b[bPos].value > a[aPos].value ? 0 : 2;
            }

            return use == 0;
        }
    }
}
