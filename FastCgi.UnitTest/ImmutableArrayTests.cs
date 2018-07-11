using System;
using Xunit;
using ByteArray = Grillisoft.ImmutableArray.ImmutableArray<byte>;

namespace Grillisoft.FastCgi.UnitTest
{
    public class ImmutableArrayTests
    {
        [Fact]
        public void Create()
        {
            var source = new byte[] { 1, 2, 3 };
            var data = new ByteArray(source);

            Assert.True(data.Count == source.Length);
            Assert.True(data.Equals(source));
        }

        [Fact]
        public void CopyTo()
        {
            var source = new byte[] { 1, 2, 3 };
            var data = new ByteArray(source);
            var dest = new byte[3];

            data.CopyTo(dest, 0);

            for (int i = 0; i < source.Length; i++)
                Assert.Equal(source[i], dest[i]);
        }

        [Fact]
        public void Concat()
        {
            var source1 = new byte[] { 1, 2, 3 };
            var source2 = new byte[] { 4, 5, 6 };

            var data = new ByteArray(source1).Concat(new ByteArray(source2));

            for (int i = 0; i < source1.Length; i++)
                Assert.Equal(source1[i], data[i]);

            for (int i = 0; i < source2.Length; i++)
                Assert.Equal(source2[i], data[i + source1.Length]);
        }

        [Fact]
        public void SubArray()
        {
            var source = new byte[] { 1, 2, 3, 4, 5, 6 };
            var data = new ByteArray(source).SubArray(2, 2, true);
            var dest = new byte[] { 3, 4 };
            var wrong = new byte[] { 5, 6 };

            Assert.True(data.Equals(dest));
            Assert.False(data.Equals(wrong));
        }

        [Fact]
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

            Assert.True(data2.Count == ret.Length);
            Assert.True(data2.Equals(ret));

            Assert.True(data2[3] == ret[3]);
            Assert.True(data2[4] != ret[3]);
        }

        [Fact]
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
                Assert.True(dataSub.Equals(dest));
            }
        }

        [Fact]
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

            Assert.Equal(dest.Length, check.Length);
            for (int i = 0; i < check.Length; i++)
                Assert.Equal(dest[i], check[i]);
        }

        [Fact]
        public void BufferManager()
        {
            var source = new byte[17342];
            for(int i=0; i < source.Length; i++)
            {
                source[i] = (byte) (i % 255);
            }

            var data = new ByteArray(source);

            Assert.True(data.Count == source.Length);
            Assert.True(data.Equals(source));
        }
    }
}
