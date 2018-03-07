using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of delegates for the randomizer.</summary>
    [SuppressMessage("Sonar", "S1144:RemoveUnusedPrivateMethod", Justification = "Used via reflection.")]
    [SuppressMessage("Sonar", "S2436:ReduceGenericParameters", Justification = "Must match any number.")]
    [SuppressMessage("Sonar", "S1186:MethodsShouldNotBeEmpty", Justification = "No behavior intended.")]
    public sealed class DelegateCreateHint : CreateHint
    {
        /// <summary>Tries to create a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>If the type could be created and the created instance.</returns>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            if (type == typeof(Delegate))
            {
                return (true, Create(typeof(Action), randomizer));
            }
            else if (type.Inherits<Delegate>())
            {
                return (true, Create(type, randomizer));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Creates a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>Created instance.</returns>
        private static object Create(Type type, RandomizerChainer randomizer)
        {
            Delegator delegator = new Delegator(randomizer);

            MethodInfo info = type.GetMethod("Invoke");
            bool hasReturn = info.ReturnType != typeof(void);

            MethodInfo match = typeof(Delegator)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.Name == "AutoDelegate")
                .Where(m => m.GetParameters().Length == info.GetParameters().Length)
                .Single(m => (m.ReturnType != typeof(void)) == hasReturn);

            IEnumerable<Type> generics = info.GetParameters().Select(p => p.ParameterType);
            if (!match.IsGenericMethod)
            {
                return Delegate.CreateDelegate(type, delegator, match);
            }
            else if (hasReturn)
            {
                return Delegate.CreateDelegate(type, delegator,
                    match.MakeGenericMethod(generics.Append(info.ReturnType).ToArray()));
            }
            else
            {
                return Delegate.CreateDelegate(type, delegator,
                    match.MakeGenericMethod(generics.ToArray()));
            }
        }

        /// <summary>Provides delegate implementations.</summary>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "For binding.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "For binding.")]
        private sealed class Delegator
        {
            /// <summary>Creates random results.</summary>
            private readonly RandomizerChainer m_Randomizer;

            /// <summary>Sets up the randomization.</summary>
            /// <param name="randomizer">Creates random results.</param>
            internal Delegator(RandomizerChainer randomizer)
            {
                m_Randomizer = randomizer;
            }

            private void AutoDelegate() { }
            private void AutoDelegate<T1>(T1 in1) { }
            private void AutoDelegate<T1, T2>(T1 in1, T2 in2) { }
            private void AutoDelegate<T1, T2, T3>(T1 in1, T2 in2, T3 in3) { }
            private void AutoDelegate<T1, T2, T3, T4>(T1 in1, T2 in2, T3 in3, T4 in4) { }
            private void AutoDelegate<T1, T2, T3, T4, T5>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14, T15 in15) { }
            private void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14, T15 in15, T16 in16) { }
            private TOut AutoDelegate<TOut>() => (TOut)m_Randomizer.Create(typeof(TOut), typeof(Delegate));
            private TOut AutoDelegate<T1, TOut>(T1 in1) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, TOut>(T1 in1, T2 in2) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, TOut>(T1 in1, T2 in2, T3 in3) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, TOut>(T1 in1, T2 in2, T3 in3, T4 in4) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14, T15 in15) => AutoDelegate<TOut>();
            private TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14, T15 in15, T16 in16) => AutoDelegate<TOut>();
        }
    }
}
