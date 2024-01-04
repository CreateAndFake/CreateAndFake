using System;
using System.Diagnostics.CodeAnalysis;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFakeTests.Toolbox.TesterTool.TestSamples;

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CS9113 // Parameter is unread

/// <summary>For testing.</summary>
[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "For testing.")]
public sealed class MockDisposableSample(object value) : IDisposable
{
    /// <summary>For testing.</summary>
    internal static readonly object _Lock = new();

    /// <summary>For testing.</summary>
    internal static Fake<IDisposable> _Fake = Tools.Faker.Stub<IDisposable>();

    /// <summary>For testing.</summary>
    internal static int _ClassDisposes = 0;

    /// <summary>For testing.</summary>
    internal static int _FinalizerDisposes = 0;

    /// <summary>For testing.</summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Must ignore in finalizer.")]
    ~MockDisposableSample()
    {
        try
        {
            _FinalizerDisposes++;
        }
        catch { }
    }

    /// <summary>For testing.</summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _ClassDisposes++;
    }

    /// <summary>For testing.</summary>
    public IDisposable DisposePass(object value)
    {
        return _Fake.Dummy;
    }
}

#pragma warning restore CS9113 // Parameter is unread
#pragma warning restore IDE0060 // Remove unused parameter
