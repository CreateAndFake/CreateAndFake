using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of common types for the randomizer.</summary>
    public sealed class CommonSystemCreateHint : CreateHint
    {
        /// <summary>Supported types and the methods used to generate them.</summary>
        private static readonly IDictionary<Type, Func<RandomizerChainer, object>> _Gens
            = new Dictionary<Type, Func<RandomizerChainer, object>>
            {
                { typeof(CultureInfo), rand => rand.Gen.NextItem(CultureInfo.GetCultures(CultureTypes.AllCultures)) },
                { typeof(TimeSpan), rand => new TimeSpan(rand.Gen.Next<long>()) },
                { typeof(DateTime), rand => new DateTime(rand.Gen.Next(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks)) },
                { typeof(Guid), rand => new Guid(Enumerable.Range(0, 16).Select(i => rand.Create<byte>()).ToArray()) },

                { typeof(Assembly), rand => rand.Gen.NextItem(AppDomain.CurrentDomain.GetAssemblies()) },
                { typeof(AssemblyName), rand => rand.Create<Assembly>().GetName() },
                { typeof(Type).GetType(), rand => rand.Create<Type>() },
                { typeof(Type), rand => rand.Gen.NextItem(Assembly.GetExecutingAssembly().GetTypes()) },

                { typeof(ConstructorInfo), rand => FindTypeInfo(rand, t => t.GetConstructors()) },
                { typeof(PropertyInfo), rand => FindTypeInfo(rand, t => t.GetProperties()) },
                { typeof(MethodInfo), rand => FindTypeInfo(rand, t => t.GetMethods()) },
                { typeof(MemberInfo), rand => FindTypeInfo(rand, t => t.GetMembers()) },
                { typeof(FieldInfo), rand => FindTypeInfo(rand, t => t.GetFields()) },
                { typeof(ParameterInfo), rand => FindTypeInfo(rand,
                    t => t.GetMethods().SelectMany(m => m.GetParameters()).ToArray()) },
                { typeof(MethodBase), rand => FindTypeInfo(rand,
                    t => t.GetConstructors().Cast<MethodBase>().Concat(t.GetMethods()).ToArray()) },
            };

        /// <summary>Tries to create a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>If the type could be created and the created instance.</returns>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            if (_Gens.TryGetValue(type, out Func<RandomizerChainer, object> gen))
            {
                return (true, gen.Invoke(randomizer));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Finds a random member info.</summary>
        /// <typeparam name="T">Type being found.</typeparam>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <param name="grabber">How members are found on a type.</param>
        /// <returns>The found member.</returns>
        private static T FindTypeInfo<T>(RandomizerChainer randomizer, Func<Type, T[]> grabber)
        {
            T[] result;
            do
            {
                result = grabber.Invoke(randomizer.Create<Type>(typeof(T)));
            } while (result.Length == 0);

            return randomizer.Gen.NextItem(result);
        }
    }
}
