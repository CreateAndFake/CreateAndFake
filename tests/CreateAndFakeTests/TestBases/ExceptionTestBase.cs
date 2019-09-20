using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using CreateAndFake;
using Xunit;

namespace CreateAndFakeTests.TestBases
{
    /// <summary>Handles testing exceptions.</summary>
    /// <typeparam name="T">Exception type to test.</typeparam>
    public abstract class ExceptionTestBase<T> where T : Exception
    {
        /// <summary>Verifies the default constructor is present for serialization but private.</summary>
        [Fact]
        public void Exception_DefaultConstructorPrivate()
        {
            Tools.Asserter.Is(null, typeof(T).GetConstructor(Type.EmptyTypes));
            Tools.Asserter.IsNot(null, Activator.CreateInstance(typeof(T), true));
        }

        /// <summary>Verifies that the exception can make a binary roundtrip.</summary>
        [Theory, RandomData]
        public void Exception_BinarySerializes(T original)
        {
            IFormatter formatter = new BinaryFormatter();

            using (Stream stream = new MemoryStream())
            {
                formatter.Serialize(stream, original);
                stream.Seek(0, SeekOrigin.Begin);

                Tools.Asserter.Is(original, formatter.Deserialize(stream));
            }
        }

        /// <summary>Verifies that the exception can make an xml roundtrip.</summary>
        [Theory, RandomData]
        public void Exception_XmlSerializes(T original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            XmlObjectSerializer formatter = new DataContractSerializer(typeof(T),
                new[] { original.InnerException }.Where(t => t != null).Select(t => t.GetType()));

            using (Stream stream = new MemoryStream())
            {
                formatter.WriteObject(stream, original);
                stream.Seek(0, SeekOrigin.Begin);

                Tools.Asserter.Is(original, formatter.ReadObject(stream));
            }
        }

        /// <summary>Verifies that the exception can make a json roundtrip.</summary>
        [Fact]
        public void Exception_JsonSerializes()
        {
            T original;
            do
            {
                original = Tools.Randomizer.Create<T>();
            } while (original.InnerException != null);

            XmlObjectSerializer formatter = new DataContractJsonSerializer(typeof(T));

            using (Stream stream = new MemoryStream())
            {
                formatter.WriteObject(stream, original);
                stream.Seek(0, SeekOrigin.Begin);

                Tools.Asserter.Is(original, formatter.ReadObject(stream));
            }
        }
    }
}
