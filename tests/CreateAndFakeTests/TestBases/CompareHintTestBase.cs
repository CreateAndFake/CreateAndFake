using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.TestBases
{
    /// <summary>Handles testing compare hints.</summary>
    /// <typeparam name="T">Compare hint to test.</typeparam>
    public abstract class CompareHintTestBase<T> where T : CompareHint
    {
        /// <summary>Instance to test with.</summary>
        protected T TestInstance { get; }

        /// <summary>Types that can be compared by the hint.</summary>
        private readonly IEnumerable<Type> _validTypes;

        /// <summary>Types that can't be compared by the hint.</summary>
        private readonly IEnumerable<Type> _invalidTypes;

        /// <summary>Sets up the tests.</summary>
        /// <param name="testInstance">Instance to test with.</param>
        /// <param name="validTypes">Types that can be compared by the hint.</param>
        /// <param name="invalidTypes">Types that can't be compared by the hint.</param>
        protected CompareHintTestBase(T testInstance, IEnumerable<Type> validTypes, IEnumerable<Type> invalidTypes)
        {
            TestInstance = testInstance;
            _validTypes = validTypes ?? Type.EmptyTypes;
            _invalidTypes = invalidTypes ?? Type.EmptyTypes;
        }

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public void CompareHint_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(TestInstance);
        }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Fact]
        public void TryCompare_SupportsSameValidTypes()
        {
            foreach (Type type in _validTypes)
            {
                object data = null;
                try
                {
                    data = Tools.Randomizer.Create(type);

                    (bool, IEnumerable<Difference>) result = TestInstance.TryCompare(data, data, CreateChainer());

                    Tools.Asserter.Is(true, result.Item1,
                        $"Hint '{typeof(T).Name}' failed to support '{type.Name}'.");
                    Tools.Asserter.IsEmpty(result.Item2,
                        $"Hint '{typeof(T).Name}' found differences with same '{type.Name}' of '{data.GetType()}'.");
                }
                catch (Exception e)
                {
                    ExpandReflectionException(e);
                    throw;
                }
                finally
                {
                    (data as IDisposable)?.Dispose();
                }
            }
        }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Fact]
        public virtual void TryCompare_SupportsDifferentValidTypes()
        {
            foreach (Type type in _validTypes)
            {
                object one = null, two = null;
                try
                {
                    one = Tools.Randomizer.Create(type);
                    two = Tools.Mutator.Variant(one.GetType(), one);

                    (bool, IEnumerable<Difference>) result = TestInstance.TryCompare(one, two, CreateChainer());

                    Tools.Asserter.Is(true, result.Item1,
                        $"Hint '{typeof(T).Name}' failed to support '{type.Name}'.");
                    Tools.Asserter.IsNotEmpty(result.Item2.ToArray(),
                        $"Hint '{typeof(T).Name}' didn't find differences with two random '{type.Name}'.");
                }
                catch (Exception e)
                {
                    ExpandReflectionException(e);
                    throw;
                }
                finally
                {
                    (one as IDisposable)?.Dispose();
                    (two as IDisposable)?.Dispose();
                }
            }
        }

        /// <summary>Verifies the hint doesn't support the wrong types.</summary>
        [Fact]
        public void TryCompare_InvalidTypesFail()
        {
            foreach (Type type in _invalidTypes)
            {
                object one = null, two = null;
                try
                {
                    one = Tools.Randomizer.Create(type);
                    two = Tools.Randomizer.Create(one.GetType());

                    Tools.Asserter.Is((false, (IEnumerable<Difference>)null),
                        TestInstance.TryCompare(one, two, CreateChainer()),
                        $"Hint '{typeof(T).Name}' should not support type '{type.Name}'.");
                }
                catch (Exception e)
                {
                    ExpandReflectionException(e);
                    throw;
                }
                finally
                {
                    (one as IDisposable)?.Dispose();
                    (two as IDisposable)?.Dispose();
                }
            }
        }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Fact]
        public void TryGetHashCode_SupportsSameValidTypes()
        {
            foreach (Type type in _validTypes)
            {
                object data = null, dataCopy = null;
                try
                {
                    data = Tools.Randomizer.Create(type);
                    dataCopy = Tools.Duplicator.Copy(data);

                    (bool, int) dataHash = TestInstance.TryGetHashCode(data, CreateChainer());
                    Tools.Asserter.Is(true, dataHash.Item1,
                        $"Hint '{typeof(T).Name}' failed to support '{type.Name}'.");
                    Tools.Asserter.Is(dataHash, TestInstance.TryGetHashCode(data, CreateChainer()),
                        $"Hint '{typeof(T).Name}' generated different hash for same '{type.Name}'.");
                    Tools.Asserter.Is(dataHash, TestInstance.TryGetHashCode(dataCopy, CreateChainer()),
                        $"Hint '{typeof(T).Name}' generated different hash for dupe '{type.Name}'.");
                }
                catch (Exception e)
                {
                    ExpandReflectionException(e);
                    throw;
                }
                finally
                {
                    (data as IDisposable)?.Dispose();
                    (dataCopy as IDisposable)?.Dispose();
                }
            }
        }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Fact]
        public void TryGetHashCode_SupportsDifferentValidTypes()
        {
            foreach (Type type in _validTypes)
            {
                object data = null, dataDiffer = null;
                try
                {
                    data = Tools.Randomizer.Create(type);
                    dataDiffer = Tools.Mutator.Variant(data);

                    (bool, int) dataHash = TestInstance.TryGetHashCode(data, CreateChainer());
                    Tools.Asserter.Is(true, dataHash.Item1,
                        $"Hint '{typeof(T).Name}' failed to support '{type.Name}'.");
                    Tools.Asserter.IsNot(dataHash, TestInstance.TryGetHashCode(dataDiffer, CreateChainer()),
                        $"Hint '{typeof(T).Name}' generated same hash for different '{type.Name}'.");
                }
                catch (Exception e)
                {
                    ExpandReflectionException(e);
                    throw;
                }
                finally
                {
                    (data as IDisposable)?.Dispose();
                    (dataDiffer as IDisposable)?.Dispose();
                }
            }
        }

        /// <summary>Verifies the hint doesn't support the wrong types.</summary>
        [Fact]
        public void TryGetHashCode_InvalidTypesFail()
        {
            foreach (Type type in _invalidTypes)
            {
                object data = null;
                try
                {
                    data = Tools.Randomizer.Create(type);

                    Tools.Asserter.Is((false, default(int)),
                        TestInstance.TryGetHashCode(data, CreateChainer()),
                        $"Hint '{typeof(T).Name}' should not support type '{type.Name}'.");
                }
                catch (Exception e)
                {
                    ExpandReflectionException(e);
                    throw;
                }
                finally
                {
                    (data as IDisposable)?.Dispose();
                }
            }
        }

        /// <returns>Chainer to use for testing.</returns>
        protected static ValuerChainer CreateChainer()
        {
            return new ValuerChainer(Tools.Valuer,
                (o, c) => Tools.Valuer.GetHashCode(o),
                (e, a, c) => Tools.Valuer.Compare(e, a));
        }

        private void ExpandReflectionException(Exception ex)
        {
            if (ex is ReflectionTypeLoadException refEx)
            {
                throw new InvalidOperationException(
                    "Reflection failure:" + refEx.LoaderExceptions.Select(e => e.Message), ex);
            }
            else if (ex.InnerException is ReflectionTypeLoadException refExInner)
            {
                throw new InvalidOperationException(
                    "Reflection failure:" + refExInner.LoaderExceptions.Select(e => e.Message), ex);
            }
        }
    }
}
