﻿using System;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Verifies behavior.</summary>
public sealed class CommonSystemCopyHintTests : CopyHintTestBase<CommonSystemCopyHint>
{
    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = [typeof(TimeSpan), typeof(WeakReference), typeof(Uri)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public CommonSystemCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }

    [Theory, RandomData]
    internal static void TryCopy_HandlesMemberInfo(MemberInfo data)
    {
        (bool, object) result = new CommonSystemCopyHint().TryCopy(data, CreateChainer());

        Tools.Asserter.Is(true, result.Item1);
        Tools.Asserter.ReferenceEqual(data, result.Item2);
    }
}
