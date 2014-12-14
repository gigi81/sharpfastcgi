using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ByteArray = FastCgi.ImmutableArray.ImmutableArray<byte>;

namespace FastCgi.UnitTest
{
    [TestClass]
    public class ImmutableArrayTests
    {
        [TestMethod]
        public void Create()
        {
            var source = new byte[] { 1, 2, 3 };
            var data = new ByteArray(source);

            Assert.IsTrue(data.Count == source.Length);
            Assert.IsTrue(data.Equals(source));
        }

        [TestMethod]
        public void CopyTo()
        {
            var source = new byte[] { 1, 2, 3 };
            var data = new ByteArray(source);
            var dest = new byte[3];

            data.CopyTo(dest, 0);

            for (int i = 0; i < source.Length; i++)
                Assert.AreEqual(source[i], dest[i]);
        }

        [TestMethod]
        public void Concat()
        {
            var source1 = new byte[] { 1, 2, 3 };
            var source2 = new byte[] { 4, 5, 6 };

            var data = new ByteArray(source1).Concat(new ByteArray(source2));

            for (int i = 0; i < source1.Length; i++)
                Assert.AreEqual(source1[i], data[i]);

            for (int i = 0; i < source2.Length; i++)
                Assert.AreEqual(source2[i], data[i + source1.Length]);
        }

        [TestMethod]
        public void SubArray()
        {
            var source = new byte[] { 1, 2, 3, 4, 5, 6 };
            var data = new ByteArray(source).SubArray(2, 2, true);
            var dest = new byte[] { 3, 4 };
            var wrong = new byte[] { 5, 6 };

            Assert.IsTrue(data.Equals(dest));
            Assert.IsFalse(data.Equals(wrong));
        }

        [TestMethod]
        public void ConcatSubArray()
        {
            var source1 = new byte[] { 1, 2, 3 };
            var source2 = new byte[] { 4, 5, 6 };
            var source3 = new byte[] { 7, 8, 9 };

            var data = new ByteArray(source1)
                               .Concat(new ByteArray(source2))
                               .Concat(new ByteArray(source3));

            var data2 = data.SubArray(2, 5);
            var ret = new byte[] { 3, 4, 5, 6, 7 };

            Assert.IsTrue(data2.Count == ret.Length);
            Assert.IsTrue(data2.Equals(ret));

            Assert.IsTrue(data2[3] == ret[3]);
            Assert.IsTrue(data2[4] != ret[3]);
        }

        [TestMethod]
        public void ConcatCopyTo()
        {
            var source1 = new byte[] { 1, 2, 3 };
            var source2 = new byte[] { 4, 5, 6 };
            var source3 = new byte[] { 7, 8, 9 };

            var data = new ByteArray(source1)
                               .Concat(new ByteArray(source2))
                               .Concat(new ByteArray(source3));

            var dest = new byte[7];

            data.CopyTo(dest, 0, dest.Length);

            using(var dataSub = data.SubArray(0, dest.Length))
            {
                Assert.IsTrue(dataSub.Equals(dest));
            }
        }

        [TestMethod]
        public void ToArray()
        {
            var source1 = new byte[] { 1, 2, 3 };
            var source2 = new byte[] { 4, 5, 6 };
            var source3 = new byte[] { 7, 8, 9 };

            var data = new ByteArray(source1)
                               .Concat(new ByteArray(source2))
                               .Concat(new ByteArray(source3));

            var dest = data.ToArray(2, 5);
            var check = new byte[] { 3, 4, 5, 6, 7 };

            Assert.AreEqual(dest.Length, check.Length);
            for (int i = 0; i < check.Length; i++)
                Assert.AreEqual(dest[i], check[i]);
        }

        [TestMethod]
        public void BufferManager()
        {
            var source = new byte[17342];
            for(int i=0; i < source.Length; i++)
            {
                source[i] = (byte) (i % 255);
            }

            var data = new ByteArray(source);

            Assert.IsTrue(data.Count == source.Length);
            Assert.IsTrue(data.Equals(source));
        }
    }
}
