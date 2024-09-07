using System;
using System.Collections.Generic;

using LiveSplit.ComponentUtil;

using Xunit;

using static LiveSplit.ComponentUtil.DeepPointer;

namespace LiveSplit.Tests.ComponentUtil;

public class DeepPointerTests
{
    // List of default exceptions when invalid arguments are supplied
    [Fact]
    public void ThrowException_WhenInitializedWithValidBaseAndNullOffsets()
    {
        Assert.Throws<NullReferenceException>(() => new DeepPointer(0, null));
    }

    [Fact]
    public void ThrowException_WhenInitializedWithValidModuleAndBaseAndNullOffsets()
    {
        Assert.Throws<NullReferenceException>(() => new DeepPointer(string.Empty, 0, null));
    }

    [Theory]
    [MemberData(nameof(DerefTypeFeeder))]
    public void ThrowException_WhenInitializedWithValidBaseAndDerefTypeButNullOffsets(DerefType anyDerefType)
    {
        Assert.Throws<NullReferenceException>(() => new DeepPointer(0, anyDerefType, null));
    }

    public static IEnumerable<object[]> DerefTypeFeeder()
    {
        foreach (object enumType in Enum.GetValues(typeof(DerefType)))
        {
            yield return new object[] { enumType };
        }
    }

    [Fact]
    public void ThrowException_WhenInitializedWithValidModuleBaseDerefTypeButNullOffsets()
    {
        Assert.Throws<NullReferenceException>(() => new DeepPointer(string.Empty, 0, DerefType.Auto, null));
    }

    [Fact]
    public void ThrowException_WhenInitializedWithValidPointerAndNullOffsets()
    {
        Assert.Throws<NullReferenceException>(() => new DeepPointer((IntPtr)0, null));
    }

    [Fact]
    public void ThrowException_WhenInitializedWithValidPointerDerefTypeButNullOffset()
    {
        Assert.Throws<NullReferenceException>(() => new DeepPointer((IntPtr)0, DerefType.Auto, null));
    }
}
