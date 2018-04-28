using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.TestBases
{
    /// <summary>Verifies behavior.</summary>
    public abstract class CompareHintTestBase<T> where T : CompareHint
    {
        /// <summary>Instance to test with.</summary>
        protected T TestInstance { get; }

        /// <summary>Types that can be compared by the hint.</summary>
        private readonly IEnumerable<Type> m_ValidTypes;

        /// <summary>Types that can't be compared by the hint.</summary>
        private readonly IEnumerable<Type> m_InvalidTypes;

        /// <summary>Sets up the tests.</summary>
        /// <param name="testInstance">Instance to test with.</param>
        /// <param name="validTypes">Types that can be compared by the hint.</param>
        /// <param name="invalidTypes">Types that can't be compared by the hint.</param>
        protected CompareHintTestBase(T testInstance, IEnumerable<Type> validTypes, IEnumerable<Type> invalidTypes)
        {
            TestInstance = testInstance;
            m_ValidTypes = validTypes ?? Type.EmptyTypes;
            m_InvalidTypes = invalidTypes ?? Type.EmptyTypes;
        }

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public void CompareHint_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<T>();
        }

        /// <summary>Verifies the hint handles nulls properly.</summary>
        [Fact]
        public virtual void TryCompare_NullChainerCheck()
        {
            object one = Tools.Randomizer.Create(Tools.Gen.NextItem(m_ValidTypes));
            object two = Tools.Duplicator.Copy(one);
            try
            {
                TestInstance.TryCompare(one, two, null);
            }
            catch (ArgumentNullException e)
            {
                Tools.Asserter.Is("valuer", e.ParamName);
            }
        }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Theory, RandomData]
        public void TryCompare_SupportsSameValidTypes(Fake<IValuer> valuer)
        {
            valuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Enumerable.Empty<Difference>()));
            valuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(true));

            foreach (Type type in m_ValidTypes)
            {
                object data = Tools.Randomizer.Create(type);

                (bool, IEnumerable<Difference>) result = TestInstance.TryCompare(data, data, CreateChainer());

                Tools.Asserter.Is(true, result.Item1,
                    "Hint '" + typeof(T).Name + "' failed to support '" + type.Name + "'.");
                Tools.Asserter.IsEmpty(result.Item2,
                    "Hint '" + typeof(T).Name + "' found differences with the same '" + type.Name + "'.");
            }
        }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Fact]
        public virtual void TryCompare_SupportsDifferentValidTypes()
        {
            Fake<IValuer> valuer = Tools.Faker.Mock<IValuer>();
            valuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Tools.Randomizer.Create<IEnumerable<Difference>>()));
            valuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(false));

            foreach (Type type in m_ValidTypes)
            {
                object one = Tools.Randomizer.Create(type);
                object two = Tools.Randiffer.Branch(one.GetType(), one);

                (bool, IEnumerable<Difference>) result = TestInstance.TryCompare(one, two, CreateChainer());

                Tools.Asserter.Is(true, result.Item1,
                    "Hint '" + typeof(T).Name + "' failed to support '" + type.Name + "'.");
                Tools.Asserter.IsNotEmpty(result.Item2.ToArray(),
                    "Hint '" + typeof(T).Name + "' didn't find differences with two random '" + type.Name + "'.");
            }
        }

        /// <summary>Verifies the hint doesn't support the wrong types.</summary>
        [Theory, RandomData]
        public void TryCompare_InvalidTypesFail(Fake<IValuer> valuer)
        {
            foreach (Type type in m_InvalidTypes)
            {
                object one = Tools.Randomizer.Create(type);
                object two = Tools.Randomizer.Create(one.GetType());

                Tools.Asserter.Is((false, (IEnumerable<Difference>)null),
                    TestInstance.TryCompare(one, two, CreateChainer()),
                    "Hint '" + typeof(T).Name + "' should not support type '" + type.Name + "'.");
                valuer.Verify(Times.Never);
            }
        }

        /// <summary>Verifies the hint handles nulls properly.</summary>
        [Fact]
        public virtual void TryGetHashCode_NullBehaviorCheck()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => TestInstance.TryGetHashCode(null, CreateChainer()));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => TestInstance.TryGetHashCode(new object(), null));
        }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Theory, RandomData]
        public void TryGetHashCode_SupportsSameValidTypes(Fake<IValuer> valuer)
        {
            valuer.Setup(
                m => m.GetHashCode(Arg.Any<object>()),
                Behavior.Returns(Tools.Randomizer.Create<int>()));

            foreach (Type type in m_ValidTypes)
            {
                object data = null, dataCopy = null;
                try
                {
                    data = Tools.Randomizer.Create(type);
                    dataCopy = Tools.Duplicator.Copy(data);

                    (bool, int) dataHash = TestInstance.TryGetHashCode(data, CreateChainer());
                    Tools.Asserter.Is(true, dataHash.Item1,
                        "Hint '" + typeof(T).Name + "' failed to support '" + type.Name + "'.");
                    Tools.Asserter.Is(dataHash, TestInstance.TryGetHashCode(data, CreateChainer()),
                        "Hint '" + typeof(T).Name + "' generated different hash for same '" + type.Name + "'.");
                    Tools.Asserter.Is(dataHash, TestInstance.TryGetHashCode(dataCopy, CreateChainer()),
                        "Hint '" + typeof(T).Name + "' generated different hash for dupe '" + type.Name + "'.");
                }
                finally
                {
                    (data as IDisposable)?.Dispose();
                    (dataCopy as IDisposable)?.Dispose();
                }
            }
        }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Theory, RandomData]
        public void TryGetHashCode_SupportsDifferentValidTypes(Fake<IValuer> valuer)
        {
            valuer.Setup(
                m => m.GetHashCode(Arg.Any<object>()),
                Behavior.Set(() => Tools.Randomizer.Create<int>()));

            foreach (Type type in m_ValidTypes)
            {
                object data = null, dataDiffer = null;
                try
                {
                    data = Tools.Randomizer.Create(type);
                    dataDiffer = Tools.Randiffer.Branch(data);

                    (bool, int) dataHash = TestInstance.TryGetHashCode(data, CreateChainer());
                    Tools.Asserter.Is(true, dataHash.Item1,
                        "Hint '" + typeof(T).Name + "' failed to support '" + type.Name + "'.");
                    Tools.Asserter.IsNot(dataHash, TestInstance.TryGetHashCode(dataDiffer, CreateChainer()),
                        "Hint '" + typeof(T).Name + "' generated same hash for different '" + type.Name + "'.");
                }
                finally
                {
                    (data as IDisposable)?.Dispose();
                    (dataDiffer as IDisposable)?.Dispose();
                }
            }
        }

        /// <summary>Verifies the hint doesn't support the wrong types.</summary>
        [Theory, RandomData]
        public void TryGetHashCode_InvalidTypesFail(Fake<IValuer> valuer)
        {
            foreach (Type type in m_InvalidTypes)
            {
                Tools.Asserter.Is((false, default(int)),
                    TestInstance.TryGetHashCode(Tools.Randomizer.Create(type), CreateChainer()),
                    "Hint '" + typeof(T).Name + "' should not support type '" + type.Name + "'.");
                valuer.Verify(Times.Never);
            }
        }

        /// <returns>Chainer to use for testing.</returns>
        protected static ValuerChainer CreateChainer()
        {
            return new ValuerChainer(Tools.Valuer,
                (o, c) => Tools.Valuer.GetHashCode(o),
                (e, a, c) => Tools.Valuer.Compare(e, a));
        }
    }
}
