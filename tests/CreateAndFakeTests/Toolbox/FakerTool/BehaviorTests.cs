using System;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class BehaviorTests
    {
        [Fact]
        internal static void Set_BehaviorWorks()
        {
            foreach (MethodInfo info in typeof(Behavior)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == nameof(Behavior.Set)))
            {
                Type type = info.GetParameters().First().ParameterType;
                Type[] generics = type.AsGenericType()?.GetGenericArguments()
                    .Select(a => typeof(string)).ToArray() ?? Type.EmptyTypes;

                MethodInfo caller = (generics.Any())
                    ? info.MakeGenericMethod(generics)
                    : info;

                Type setupType = caller.GetParameters().First().ParameterType;

                Type[] args = (type.Name.StartsWith("Func", StringComparison.InvariantCulture))
                    ? generics.Skip(1).ToArray()
                    : generics;

                Behavior noTimes = (Behavior)caller.Invoke(null,
                    new[] { Tools.Randomizer.Create(setupType), null });

                Tools.Asserter.Is(false, noTimes.HasExpectedCalls());
                noTimes.Invoke(args.Select(g => Tools.Randomizer.Create(g)).ToArray());
                Tools.Asserter.Is(true, noTimes.HasExpectedCalls());

                Behavior withTimes = (Behavior)caller.Invoke(null,
                    new[] { Tools.Randomizer.Create(setupType), Times.Never });

                Tools.Asserter.Is(true, withTimes.HasExpectedCalls());
                withTimes.Invoke(args.Select(g => Tools.Randomizer.Create(g)).ToArray());
                Tools.Asserter.Is(false, withTimes.HasExpectedCalls());
            }
        }

        [Fact]
        internal static void None_BehaviorWorks()
        {
            Behavior.None().Invoke(Array.Empty<object>());
        }

        [Fact]
        internal static void Error_BehaviorWorks()
        {
            Tools.Asserter.Throws<NotImplementedException>(
                () => Behavior.Error().Invoke(Array.Empty<object>()));
        }

        [Fact]
        internal static void Null_BehaviorWorks()
        {
            Tools.Asserter.Is(null, Behavior.Null<string>().Invoke(Array.Empty<object>()));
        }

        [Fact]
        internal static void Default_BehaviorWorks()
        {
            Tools.Asserter.Is(default(int), Behavior.Default<int>().Invoke(Array.Empty<object>()));
        }

        [Fact]
        internal static void Throw_BehaviorWorks()
        {
            Tools.Asserter.Throws<InvalidOperationException>(
                () => Behavior.Throw<InvalidOperationException>().Invoke(Array.Empty<object>()));
        }

        [Theory, RandomData]
        internal static void Returns_BehaviorWorks(int value)
        {
            Tools.Asserter.Is(value, Behavior.Returns(value).Invoke(Array.Empty<object>()));
        }

        [Fact]
        internal static void Series_BehaviorWorks()
        {
            Behavior behavior = Behavior.Series(false, true, false);
            Tools.Asserter.Is(false, behavior.Invoke(Array.Empty<object>()));
            Tools.Asserter.Is(true, behavior.Invoke(Array.Empty<object>()));
            Tools.Asserter.Is(false, behavior.Invoke(Array.Empty<object>()));
            Tools.Asserter.Is(false, behavior.Invoke(Array.Empty<object>()));
        }

        [Theory, RandomData]
        internal static void Series_NullCountsAsNone(string value)
        {
            Behavior behavior = Behavior.Series(value, null);
            Tools.Asserter.Is(value, behavior.Invoke(Array.Empty<object>()));

            Tools.Asserter.Is(null, behavior.Invoke(Array.Empty<object>()));
        }

        [Theory, RandomData]
        internal static void ToExpectedCalls_MatchesTimes(Times times)
        {
            Tools.Asserter.Is(Times.Min(1).ToString(), Behavior.None().ToExpectedCalls());
            Tools.Asserter.Is(times.ToString(), Behavior.None(times).ToExpectedCalls());
        }

        [Fact]
        internal static void Invoke_ThrowsWithWrongArgs()
        {
            Tools.Asserter.Throws<TargetParameterCountException>(
                () => Behavior.Set((int _) => { }).Invoke(Array.Empty<object>()));
        }
    }
}
