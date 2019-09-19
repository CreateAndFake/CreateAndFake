using System;
using System.Diagnostics.CodeAnalysis;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFakeTests.Toolbox.TesterTool.TestSamples
{
    /// <summary>For testing.</summary>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "For testing.")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "For testing.")]
    [SuppressMessage("Sonar", "S2696:StaticModification", Justification = "For testing.")]
    [SuppressMessage("Sonar", "S2223:FieldVisibility", Justification = "For testing.")]
    public sealed class MockDisposableSample : IDisposable
    {
        /// <summary>For testing.</summary>
        internal static readonly object _Lock = new object();

        /// <summary>For testing.</summary>
        internal static Fake<IDisposable> _Fake = Tools.Faker.Stub<IDisposable>();

        /// <summary>For testing.</summary>
        internal static int _ClassDisposes = 0;

        /// <summary>For testing.</summary>
        internal static int _FinalizerDisposes = 0;

        /// <summary>For testing.</summary>
        [SuppressMessage("IDE", "IDE0060:RemoveUnusedParameters", Justification = "For testing.")]
        public MockDisposableSample(object value) { }

        /// <summary>For testing.</summary>
        ~MockDisposableSample()
        {
            _FinalizerDisposes++;
        }

        /// <summary>For testing.</summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _ClassDisposes++;
        }

        /// <summary>For testing.</summary>
        [SuppressMessage("IDE", "IDE0060:RemoveUnusedParameters", Justification = "For testing.")]
        public IDisposable DisposePass(object value)
        {
            return _Fake.Dummy;
        }
    }
}
