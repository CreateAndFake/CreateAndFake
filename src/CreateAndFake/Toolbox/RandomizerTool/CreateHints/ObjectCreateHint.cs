using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of objects in general.</summary>
    public sealed class ObjectCreateHint : CreateHint
    {
        /// <summary>Caches found subclasses for types.</summary>
        private static readonly IDictionary<Type, Type[]> s_SubclassCache = new Dictionary<Type, Type[]>
        {
            { typeof(object), Array.Empty<Type>() }
        };

        /// <summary>Tries to create a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>If the type could be created and the created instance.</returns>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            object result = Create(type, randomizer);
            return (result != null, result);
        }

        /// <summary>Creates a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>Created instance.</returns>
        private static object Create(Type type, RandomizerChainer randomizer)
        {
            Type newType = FindTypeToCreate(type, randomizer);
            if (newType != type)
            {
                return randomizer.Create(newType);
            }

            object data = CreateNew(type, randomizer);
            if (data == null)
            {
                return data;
            }

            Type dataType = data.GetType();

            foreach (FieldInfo field in dataType.GetFields().Where(f => !f.IsInitOnly && !f.IsLiteral))
            {
                field.SetValue(data, randomizer.Create(field.FieldType, dataType));
            }
            foreach (PropertyInfo property in dataType.GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(data, randomizer.Create(property.PropertyType, dataType));
            }

            return data;
        }

        /// <summary>Creates a new instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>Created instance.</returns>
        private static object CreateNew(Type type, RandomizerChainer randomizer)
        {
            /*
             * Order of preference:
             * 1) Default constructor.
             * 2) Public constructor.
             * 3) Public factory.
             * 4) Internal factory.
             * 5) Internal constructor.
             * 6) Stub.
             */

            ConstructorInfo defaultConstructor = type.GetConstructor(Type.EmptyTypes);
            if (type == typeof(object))
            {
                IFaked fake = randomizer.Stub<IFaked>().Dummy;
                fake.FakeMeta.Identifier = randomizer.Create<int>(type);
                return fake;
            }
            else if (defaultConstructor != null)
            {
                return defaultConstructor.Invoke(null);
            }
            else if (FindConstructors(type, BindingFlags.Public).Any())
            {
                return CreateFrom(type, randomizer, (c, d) => c.Invoke(d),
                    FindConstructors(type, BindingFlags.Public));
            }
            else if (FindFactories(type, BindingFlags.Public).Any())
            {
                return CreateFrom(type, randomizer, (c, d) => c.Invoke(null, d),
                    FindFactories(type, BindingFlags.Public));
            }
            else if (FindFactories(type, BindingFlags.NonPublic).Any())
            {
                return CreateFrom(type, randomizer, (c, d) => c.Invoke(null, d),
                    FindFactories(type, BindingFlags.NonPublic));
            }
            else if (FindConstructors(type, BindingFlags.NonPublic).Any())
            {
                return CreateFrom(type, randomizer, (c, d) => c.Invoke(d),
                    FindConstructors(type, BindingFlags.NonPublic));
            }
            else if (!type.IsSealed)
            {
                return randomizer.Stub(type).Dummy;
            }
            else
            {
                return null;
            }
        }

        /// <summary>Creates the type.</summary>
        /// <typeparam name="T">Creation method type.</typeparam>
        /// <param name="type">Type to create.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <param name="invoker">How to create the type from the creation method.</param>
        /// <param name="creators">Possible creation methods.</param>
        /// <returns>Created instance.</returns>
        private static object CreateFrom<T>(Type type, RandomizerChainer randomizer,
            Func<T, object[], object> invoker, IEnumerable<T> creators) where T : MethodBase
        {
            T creator = randomizer.Gen.NextItem(creators);
            return invoker.Invoke(creator, creator.GetParameters()
                .Select(p => randomizer.Create(p.ParameterType, type))
                .ToArray());
        }

        /// <summary>Finds a createable type of the given type.</summary>
        /// <param name="type">Parent type being created.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>Type to use.</returns>
        private static Type FindTypeToCreate(Type type, RandomizerChainer randomizer)
        {
            if (!s_SubclassCache.TryGetValue(type, out Type[] subclasses))
            {
                subclasses = FindSubclasses(type);
                s_SubclassCache.Add(type, subclasses);
            }

            IEnumerable<Type> pruned = subclasses.Where(t => !randomizer.AlreadyCreated(t));
            return pruned.Any()
                ? randomizer.Gen.NextItem(pruned)
                : type;
        }

        /// <summary>Finds subclasses of the given type.</summary>
        /// <param name="type">Parent type.</param>
        /// <returns>Found subclasses.</returns>
        private static Type[] FindSubclasses(Type type)
        {
            BindingFlags anyScope = BindingFlags.Public | BindingFlags.NonPublic;

            Type[] subclasses = type.FindLocalSubclasses()
                .Where(t => FindConstructors(t, anyScope).Any() || FindFactories(t, anyScope).Any())
                .Where(t => !t.IsNestedPrivate)
                .ToArray();

            if (subclasses.Any())
            {
                return subclasses;
            }
            else
            {
                return type.FindLoadedSubclasses()
                    .Where(t => FindConstructors(t, anyScope).Any() || FindFactories(t, anyScope).Any())
                    .Where(t => !t.IsNestedPrivate)
                    .ToArray();
            }
        }

        /// <summary>Finds public or internal constructors.</summary>
        /// <param name="type">Type to search.</param>
        /// <param name="scope">Which constructors to look for.</param>
        /// <returns>Found constructors.</returns>
        private static IEnumerable<ConstructorInfo> FindConstructors(Type type, BindingFlags scope)
        {
            return type
                .GetConstructors(BindingFlags.Instance | scope)
                .Where(c => c.IsPublic || c.IsAssembly);
        }

        /// <summary>Finds static methods that create the type.</summary>
        /// <param name="type">Type to search.</param>
        /// <param name="scope">Which constructors to look for.</param>
        /// <returns>Found factory methods.</returns>
        private static IEnumerable<MethodInfo> FindFactories(Type type, BindingFlags scope)
        {
            return type
                .GetMethods(BindingFlags.Static | scope)
                .Where(m => m.IsPublic || m.IsAssembly)
                .Where(m => m.ReturnType.Inherits(type))
                .Where(m => !m.ContainsGenericParameters);
        }
    }
}
