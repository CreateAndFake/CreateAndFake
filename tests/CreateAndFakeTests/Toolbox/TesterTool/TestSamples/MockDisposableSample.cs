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
        internal static readonly object Lock = new object();

        /// <summary>For testing.</summary>
        internal static Fake<IDisposable> Fake = Tools.Faker.Stub<IDisposable>();

        /// <summary>For testing.</summary>
        internal static int ClassDisposes = 0;

        /// <summary>For testing.</summary>
        internal static int FinalizerDisposes = 0;

        /// <summary>For testing.</summary>
        public MockDisposableSample(object value) { }

        /// <summary>For testing.</summary>
        ~MockDisposableSample()
        {
            FinalizerDisposes++;
        }

        /// <summary>For testing.</summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            ClassDisposes++;
        }

        /// <summary>For testing.</summary>
        public IDisposable DisposePass(object value)
        {
            return Fake.Dummy;
        }
    }
}
