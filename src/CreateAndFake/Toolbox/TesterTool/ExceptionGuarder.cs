using System;
using System.Reflection;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates basic layer passthrough checks.</summary>
    internal sealed class ExceptionGuarder : BaseGuarder
    {
        /// <summary>Handles common test scenarios.</summary>
        private readonly Asserter _asserter;

        /// <summary>Initializes a new instance of the <see cref="ExceptionGuarder"/> class.</summary>
        /// <param name="fixer">Handles generic resolution.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <param name="timeout">How long to wait for methods to complete.</param>
        internal ExceptionGuarder(GenericFixer fixer, IRandomizer randomizer, Asserter asserter, TimeSpan timeout)
            : base(fixer, randomizer, timeout)
        {
            _asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
        }

        /// <summary>Verifies no exceptions are thrown on any method when using injection and random data.</summary>
        /// <param name="instance">Instance to test the methods on.</param>
        /// <param name="injectionValues">Values to inject into called methods.</param>
        internal void CallAllMethods(object instance, params object[] injectionValues)
        {
            base.CallAllMethods(null, null, instance, injectionValues);
        }

        /// <summary>Handles exceptions encountered by the check.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="testParam">Parameter being set to null.</param>
        /// <param name="taskException">Exception encountered.</param>
        protected override void HandleCheckException(MethodBase testOrigin, ParameterInfo testParam, Exception taskException)
        {
            if (testOrigin == null) throw new ArgumentNullException(nameof(testOrigin));
            if (taskException == null) throw new ArgumentNullException(nameof(taskException));

            _asserter.Fail(taskException, $"Exception encountered on method '{testOrigin.Name}'.");
        }
    }
}
