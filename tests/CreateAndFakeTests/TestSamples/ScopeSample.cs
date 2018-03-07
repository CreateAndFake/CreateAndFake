using System.Diagnostics.CodeAnalysis;

namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "For testing."),
        SuppressMessage("Sonar", "S1144:RemoveUnusedProperty", Justification = "For testing.")]
    public class ScopeSample
    {
        /// <summary>For testing.</summary>
        public virtual string PublicProp { get; set; }

        /// <summary>For testing.</summary>
        internal virtual string InternalProp { get; set; }

        /// <summary>For testing.</summary>
        protected internal virtual string ProIntProp { get; set; }

        /// <summary>For testing.</summary>
        protected virtual string ProtectProp { get; set; }

        /// <summary>For testing.</summary>
        public virtual string InternalGet { internal get; set; }

        /// <summary>For testing.</summary>
        public virtual string ProIntGet { protected internal get; set; }

        /// <summary>For testing.</summary>
        public virtual string ProtectGet { protected get; set; }

        /// <summary>For testing.</summary>
        public virtual string PrivateGet { private get; set; }

        /// <summary>For testing.</summary>
        public virtual string InternalSet { get; internal set; }

        /// <summary>For testing.</summary>
        public virtual string ProIntSet { get; protected internal set; }

        /// <summary>For testing.</summary>
        public virtual string ProtectSet { get; protected set; }

        /// <summary>For testing.</summary>
        public virtual string PrivateSet { get; private set; }

        /// <summary>For testing.</summary>
        public virtual string PublicMethod() { return "Value"; }

        /// <summary>For testing.</summary>
        internal virtual string InternalMethod() { return "Value"; }

        /// <summary>For testing.</summary>
        protected internal virtual string ProIntMethod() { return "Value"; }

        /// <summary>For testing.</summary>
        /// <returns></returns>
        protected virtual string ProtectMethod() { return "Value"; }

        /// <summary>For testing.</summary>
        /// <returns>For testing.</returns>
        public string CallProtectProp()
        {
            return ProtectProp;
        }

        /// <summary>For testing.</summary>
        /// <param name="data">For testing.</param>
        public void SetProtectProp(string data)
        {
            ProtectProp = data;
        }

        /// <summary>For testing.</summary>
        /// <returns>For testing.</returns>
        public string CallProtectGet()
        {
            return ProtectGet;
        }

        /// <summary>For testing.</summary>
        /// <param name="data">For testing.</param>
        public void SetProtectSet(string data)
        {
            ProtectSet = data;
        }

        /// <summary>For testing.</summary>
        /// <returns>For testing.</returns>
        public string CallProtectMethod()
        {
            return ProtectMethod();
        }
    }
}
