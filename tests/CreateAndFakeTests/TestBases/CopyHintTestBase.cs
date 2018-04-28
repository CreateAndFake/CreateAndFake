using System;
using System.Collections.Generic;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool;
using Xunit;

namespace CreateAndFakeTests.TestBases
{
    /// <summary>Verifies behavior.</summary>
    public abstract class CopyHintTestBase<T> where T : CopyHint, new()
    {
        /// <summary>Instance to test with.</summary>
        protected T TestInstance { get; } = new T();

        /// <summary>Types that can be copied by the hint.</summary>
        private readonly IEnumerable<Type> m_ValidTypes;

        /// <summary>Types that can't be copied by the hint.</summary>
        private readonly IEnumerable<Type> m_InvalidTypes;

        /// <summary>If the hint copies by reference instead for value types.</summary>
        private readonly bool m_CopiesByRef;

        /// <summary>Sets up the tests.</summary>
        /// <param name="validTypes">Types that can be copied by the hint.</param>
        /// <param name="invalidTypes">Types that can't be copied by the hint.</param>
        /// <param name="copiesByRef">If the hint copies by reference instead for value types.</param>
        protected CopyHintTestBase(IEnumerable<Type> validTypes, IEnumerable<Type> invalidTypes, bool copiesByRef = false)
        {
            m_ValidTypes = validTypes ?? Type.EmptyTypes;
            m_InvalidTypes = invalidTypes ?? Type.EmptyTypes;
            m_CopiesByRef = copiesByRef;
        }

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public void CopyHint_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<T>();
        }

        /// <summary>Verifies the hint handles nulls properly.</summary>
        [Fact]
        public void TryCopy_NullBehaviorCheck()
        {
            Tools.Asserter.Is((true, (object)null),
                TestInstance.TryCopy(null, CreateChainer()));

            object data = Tools.Randomizer.Create(Tools.Gen.NextItem(m_ValidTypes));
            try
            {
                TestInstance.TryCopy(data, null);
            }
            catch (ArgumentNullException e)
            {
                Tools.Asserter.Is("duplicator", e.ParamName);
            }
        }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Fact]
        public void TryCopy_SupportsValidTypes()
        {
            foreach (Type type in m_ValidTypes)
            {
                object data = Tools.Randomizer.Create(type);
                (bool, object) result = TestInstance.TryCopy(data, CreateChainer());

                Tools.Asserter.Is((true, data), result,
                    "Hint '" + typeof(T).Name + "' failed to clone type '" + type.Name + "'.");

                if (m_CopiesByRef || data is string)
                {
                    Tools.Asserter.ReferenceEqual(data, result.Item2,
                        "Hint '" + typeof(T).Name + "' expected to copy value types by ref of type '" + type.Name + "'.");
                }
                else
                {
                    Tools.Asserter.ReferenceNotEqual(data, result.Item2,
                        "Hint '" + typeof(T).Name + "' copied by ref instead of a deep clone of type '" + type.Name + "'.");
                }

                (data as IDisposable)?.Dispose();
                (result.Item2 as IDisposable)?.Dispose();
            }
        }

        /// <summary>Verifies the hint doesn't support the wrong types.</summary>
        [Fact]
        public void TryCopy_InvalidTypesFail()
        {
            foreach (Type type in m_InvalidTypes)
            {
                object data = Tools.Randomizer.Create(type);
                (bool, object) result = TestInstance.TryCopy(data, CreateChainer());

                Tools.Asserter.Is((false, (object)null), result,
                    "Hint '" + typeof(T).Name + "' should not support type '" + type.Name + "'.");

                (data as IDisposable)?.Dispose();
                (result.Item2 as IDisposable)?.Dispose();
            }
        }

        /// <returns>Chainer to use for testing.</returns>
        protected static DuplicatorChainer CreateChainer()
        {
            return new DuplicatorChainer(Tools.Duplicator, (o, c) => Tools.Duplicator.Copy(o));
        }
    }
}
