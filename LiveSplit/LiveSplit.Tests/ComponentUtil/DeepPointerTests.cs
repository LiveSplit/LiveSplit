using System;
using System.Collections.Generic;
using Xunit;
using LiveSplit.ComponentUtil;
using static LiveSplit.ComponentUtil.DeepPointer;

namespace LiveSplit.Tests.ComponentUtil
{
    public class DeepPointerTests
    {
        // List of default exceptions when invalid arguments are supplied
        [Fact]
        public void ThrowException_WhenInitializedWithValidBaseAndNullOffsets() =>
            Assert.Throws<ArgumentNullException>(() => new DeepPointer(0, null));

        [Fact]
        public void ThrowException_WhenInitializedWithValidModuleAndBaseAndNullOffsets() =>
            Assert.Throws<ArgumentNullException>(() => new DeepPointer(string.Empty, 0, null));

        [Theory]
        [MemberData(nameof(DerefTypeFeeder))]
        public void ThrowException_WhenInitializedWithValidBaseAndDerefTypeButNullOffsets(DerefType anyDerefType) =>
            Assert.Throws<ArgumentNullException>(() => new DeepPointer(0, anyDerefType, null));

        public static IEnumerable<object[]> DerefTypeFeeder()
        {
            foreach (var enumType in Enum.GetValues(typeof(DerefType)))
            {
                yield return new object[] { enumType };
            }
        }

        [Fact]
        public void ThrowException_WhenInitializedWithValidModuleBaseDerefTypeButNullOffsets() =>
            Assert.Throws<ArgumentNullException>(() => new DeepPointer(string.Empty, 0, DerefType.Auto, null));

        [Fact]
        public void ThrowException_WhenInitializedWithValidPointerAndNullOffsets() =>
            Assert.Throws<ArgumentNullException>(() => new DeepPointer((IntPtr)0, null));

        [Fact]
        public void ThrowException_WhenInitializedWithValidPointerDerefTypeButNullOffset() =>
            Assert.Throws<ArgumentNullException>(() => new DeepPointer((IntPtr)0, DerefType.Auto, null));
    }
}
