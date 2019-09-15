using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of delegates for the randomizer.</summary>
    [SuppressMessage("Sonar", "S1144:RemoveUnusedPrivateMethod", Justification = "Used via reflection.")]
    [SuppressMessage("Sonar", "S2436:ReduceGenericParameters", Justification = "Must match any number.")]
    [SuppressMessage("Sonar", "S1186:MethodsShouldNotBeEmpty", Justification = "No behavior intended.")]
    public sealed class DelegateCreateHint : CreateHint
    {
        /// <summary>Methods used to match delegates.</summary>
        private static readonly MethodInfo[] _Delegators = typeof(Delegator)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.Name == "AutoDelegate")
            .ToArray();

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

            MethodInfo match = _Delegators
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
            private readonly RandomizerChainer _randomizer;

            /// <summary>Sets up the randomization.</summary>
            /// <param name="randomizer">Creates random results.</param>
            internal Delegator(RandomizerChainer randomizer)
            {
                _randomizer = randomizer;
            }

            /// <summary>Randomizes any OutRef inputs.</summary>
            /// <param name="inputs">Inputs for the delegate.</param>
            private void HandleIn(params object[] inputs)
            {
                foreach (object outRef in inputs.Where(o => o != null))
                {
                    Type inputType = outRef.GetType();
                    if (inputType.Inherits<IOutRef>())
                    {
                        FieldInfo valueField = inputType.GetField(nameof(OutRef<object>.Var));
                        valueField.SetValue(outRef, _randomizer.Create(valueField.FieldType));
                    }
                }
            }

            /// <summary>Randomizes any OutRef inputs and the return.</summary>
            /// <typeparam name="T">Return type for the delegate.</typeparam>
            /// <param name="inputs">Inputs for the delegate.</param>
            /// <returns>Generated output for the delegate.</returns>
            private T HandleInAndOut<T>(params object[] inputs)
            {
                HandleIn(inputs);
                return (T)_randomizer.Create(typeof(T), typeof(Delegate));
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate()
            {
                HandleIn();
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1>(T1 in1)
            {
                HandleIn(in1);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2>(T1 in1, T2 in2)
            {
                HandleIn(in1, in2);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3>(T1 in1, T2 in2, T3 in3)
            {
                HandleIn(in1, in2, in3);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4>(T1 in1, T2 in2, T3 in3, T4 in4)
            {
                HandleIn(in1, in2, in3, in4);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5)
            {
                HandleIn(in1, in2, in3, in4, in5);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6)
            {
                HandleIn(in1, in2, in3, in4, in5, in6);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7, in8);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7, in8, in9);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13, in14);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14, T15 in15)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13, in14, in15);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal void AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14, T15 in15, T16 in16)
            {
                HandleIn(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13, in14, in15, in16);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<TOut>()
            {
                return HandleInAndOut<TOut>();
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, TOut>(T1 in1)
            {
                return HandleInAndOut<TOut>(in1);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, TOut>(T1 in1, T2 in2)
            {
                return HandleInAndOut<TOut>(in1, in2);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, TOut>(T1 in1, T2 in2, T3 in3)
            {
                return HandleInAndOut<TOut>(in1, in2, in3);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, TOut>(T1 in1, T2 in2, T3 in3, T4 in4)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, TOut>(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, TOut>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7, in8);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOut>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7, in8, in9);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOut>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOut>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOut>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOut>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13, in14);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOut>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14, T15 in15)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13, in14, in15);
            }

            /// <summary>Matches to delegate with same number of inputs and output.</summary>
            internal TOut AutoDelegate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOut>(
                T1 in1, T2 in2, T3 in3, T4 in4, T5 in5, T6 in6, T7 in7, T8 in8, T9 in9, T10 in10, T11 in11, T12 in12, T13 in13, T14 in14, T15 in15, T16 in16)
            {
                return HandleInAndOut<TOut>(in1, in2, in3, in4, in5, in6, in7, in8, in9, in10, in11, in12, in13, in14, in15, in16);
            }
        }
    }
}
