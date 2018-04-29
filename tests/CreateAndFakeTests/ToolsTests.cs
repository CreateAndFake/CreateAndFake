using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CreateAndFake;
using CreateAndFake.Toolbox;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests
{
    /// <summary>Verifies behavior.</summary>
    public static class ToolsTests
    {
        /// <summary>Flags representing mutable data.</summary>
        private const BindingFlags s_Mutable = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>Verifies that the tools integrate together.</summary>
        [Fact]
        public static void Tools_IntegrationWorks()
        {
            DataHolderSample original = Tools.Randomizer.Create<DataHolderSample>();
            DataHolderSample variant = Tools.Randiffer.Branch(original);
            DataHolderSample dupe = Tools.Duplicator.Copy(original);

            Tools.Asserter.ValuesEqual(original, dupe);
            Tools.Asserter.ValuesNotEqual(original, variant);

            Fake<DataHolderSample> faked = Tools.Faker.Mock<DataHolderSample>();
            faked.Setup(
                m => m.HasNested(dupe),
                Behavior.Returns(true, Times.Once));

            Tools.Asserter.Is(true, faked.Dummy.HasNested(original),
                "Value equality did not work for args.");

            faked.Verify(Times.Once);
        }

        /// <summary>Verifies the tools have limits.</summary>
        [Fact]
        public static void Tools_HandlesInfinites()
        {
            Tools.Asserter.Throws<InfiniteLoopException>(
                () => Tools.Randomizer.Create<InfiniteSample>());

            InfiniteSample sample = new InfiniteSample();
            sample.Hole = sample;

            Tools.Asserter.Throws<TimeoutException>(
                () => Tools.Randiffer.Branch(sample));

            InfiniteSample dupe = Tools.Duplicator.Copy(sample);

            Tools.Asserter.Is(sample, dupe);
            Tools.Asserter.Is(Tools.Valuer.GetHashCode(sample), Tools.Valuer.GetHashCode(dupe));
        }

        /// <summary>Verifies the tools work on all the introduced types besides static classes.</summary>
        [Fact]
        public static void Tools_AllCreateAndFakeTypesWork()
        {
            Type[] ignore = new[] { typeof(Arg), typeof(Fake), typeof(Fake<>),
                typeof(VoidType), typeof(AnyGeneric), typeof(NullGuarder) };

            foreach (Type type in typeof(Tools).Assembly.GetTypes()
                .Where(t => !(t.IsAbstract && t.IsSealed))
                .Where(t => !ignore.Contains(t))
                .Where(t => !t.IsNestedPrivate)
                .Where(t => t.GetCustomAttribute<CompilerGeneratedAttribute>() == null))
            {
                TestTrip(type);
            }
        }

        /// <summary>Verifies the tools work on possible exceptions.</summary>
        [Fact]
        public static void Tools_ExceptionTypesWork()
        {
            Type type = typeof(Exception);

            for (int i = 0; i < 100; i++)
            {
                TestTrip(type);
            }
        }

        /// <summary>Verifies the type works with the tools.</summary>
        /// <param name="type">Type to test.</param>
        private static void TestTrip(Type type)
        {
            string failMessage = "Behavior did not work for type '" + type.FullName + "'.";
            object
                original = null,
                variant = null,
                dupe = null;
            try
            {
                original = Tools.Randomizer.Create(type);
                dupe = Tools.Duplicator.Copy(original);
                Tools.Asserter.ValuesEqual(original, dupe, failMessage);
                Tools.Asserter.ValuesEqual(
                    Tools.Valuer.GetHashCode(original),
                    Tools.Valuer.GetHashCode(dupe), failMessage);

                if (type.GetProperties(s_Mutable).Any() || type.GetFields(s_Mutable).Any())
                {
                    variant = Tools.Randiffer.Branch(type, original);
                    Tools.Asserter.ValuesNotEqual(original, variant, failMessage);
                    Tools.Asserter.ValuesNotEqual(
                        Tools.Valuer.GetHashCode(original),
                        Tools.Valuer.GetHashCode(variant), failMessage);
                }

                if (Tools.Faker.Supports(type) && !type.Inherits<IDisposable>())
                {
                    Tools.Faker.Mock(type);
                }
            }
            finally
            {
                (original as IDisposable)?.Dispose();
                (variant as IDisposable)?.Dispose();
                (dupe as IDisposable)?.Dispose();
            }
        }
    }
}
