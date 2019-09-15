using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

namespace CreateAndFake.Toolbox.DuplicatorTool
{
    /// <summary>Deep clones objects.</summary>
    public sealed class Duplicator : IDuplicator, IDuplicatable
    {
        /// <summary>Default set of hints to use for copying.</summary>
        private static readonly CopyHint[] s_DefaultHints = new CopyHint[]
        {
            new CommonSystemCopyHint(),
            new DeepCloneableCopyHint(),
            new DuplicatableCopyHint(),
            new BasicCopyHint(),
            new LegacyCollectionCopyHint(),
            new CollectionCopyHint(),
            new CloneableCopyHint(),
            new SerializableCopyHint(),
            new ObjectCopyHint()
        };

        /// <summary>Verifies duplicates are valid.</summary>
        private readonly Asserter m_Asserter;

        /// <summary>Hints used to copy specific types.</summary>
        private readonly IEnumerable<CopyHint> m_Hints;

        /// <summary>Sets up the duplicator capabilities.</summary>
        /// <param name="asserter">Verifies duplicates are valid.</param>
        /// <param name="includeDefaultHints">If the default set of hints should be added.</param>
        /// <param name="hints">Hints used to copy specific types.</param>
        public Duplicator(Asserter asserter, bool includeDefaultHints = true, params CopyHint[] hints)
        {
            m_Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));

            IEnumerable<CopyHint> inputHints = hints ?? Enumerable.Empty<CopyHint>();
            if (includeDefaultHints)
            {
                m_Hints = inputHints.Concat(s_DefaultHints).ToArray();
            }
            else
            {
                m_Hints = inputHints.ToArray();
            }
        }

        /// <summary>Deep clones an object.</summary>
        /// <typeparam name="T">Type being cloned.</typeparam>
        /// <param name="source">Object to clone.</param>
        /// <returns>The duplicate.</returns>
        /// <exception cref="NotSupportedException">If no hint supports cloning the object.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        public T Copy<T>(T source)
        {
            try
            {
                return Copy(source, new DuplicatorChainer(this, Copy));
            }
            catch (InsufficientExecutionStackException)
            {
                throw new InsufficientExecutionStackException(
                    $"Ran into infinite generation trying to duplicate type '{source.GetType().Name}'.");
            }
        }

        /// <summary>Deep clones an object.</summary>
        /// <typeparam name="T">Type being cloned.</typeparam>
        /// <param name="source">Object to clone.</param>
        /// <param name="chainer">Handles callback behavior for child values.</param>
        /// <returns>The duplicate.</returns>
        /// <exception cref="NotSupportedException">If no hint supports cloning the object.</exception>
        private T Copy<T>(T source, DuplicatorChainer chainer)
        {
            if (source == null) return default;

            (bool, object) result = m_Hints
                .Select(h => h.TryCopy(source, chainer))
                .FirstOrDefault(r => r.Item1);

            if (!result.Equals(default))
            {
                m_Asserter.ValuesEqual(source, result.Item2,
                    $"Type '{source.GetType()}' did not clone properly. " +
                    "Verify/create a hint to generate the type and pass it to the duplicator.");
                return (T)result.Item2;
            }
            else
            {
                throw new NotSupportedException(
                    $"Type '{source.GetType().FullName}' not supported by the duplicator. " +
                    "Create a hint to generate the type and pass it to the duplicator.");
            }
        }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <param name="duplicator">Duplicator to clone child values.</param>
        /// <returns>Clone that is equal in value to the instance.</returns>
        public IDuplicatable DeepClone(IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return new Duplicator(duplicator.Copy(m_Asserter), false, duplicator.Copy(m_Hints).ToArray());
        }
    }
}
