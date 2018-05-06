using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of common types for the randomizer.</summary>
    public sealed class CommonSystemCreateHint : CreateHint
    {
        /// <summary>Random types to use for generation.</summary>
        private static readonly Type[] s_RandomTypes = new[] { typeof(string), typeof(int), typeof(object) };

        /// <summary>Supported types and the methods used to generate them.</summary>
        private static readonly IDictionary<Type, Func<RandomizerChainer, object>> s_Gens
            = new Dictionary<Type, Func<RandomizerChainer, object>>
            {
                { typeof(Type).GetType(), rand => rand.Gen.NextItem(s_RandomTypes) },
                { typeof(Type), rand => rand.Gen.NextItem(Assembly.GetExecutingAssembly().GetTypes()) },
                { typeof(PropertyInfo), rand => FindTypeInfo(rand, t => t.GetProperties()) },
                { typeof(MethodInfo), rand => FindTypeInfo(rand, t => t.GetMethods()) },
                { typeof(MemberInfo), rand => FindTypeInfo(rand, t => t.GetMembers()) },
                { typeof(FieldInfo), rand => FindTypeInfo(rand, t => t.GetFields()) },
                { typeof(CultureInfo), rand => rand.Gen.NextItem(CultureInfo.GetCultures(CultureTypes.AllCultures)) },
                { typeof(TimeSpan), rand => new TimeSpan(rand.Gen.Next<long>()) },
            };

        /// <summary>Tries to create a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>If the type could be created and the created instance.</returns>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            if (s_Gens.TryGetValue(type, out Func<RandomizerChainer, object> gen))
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
