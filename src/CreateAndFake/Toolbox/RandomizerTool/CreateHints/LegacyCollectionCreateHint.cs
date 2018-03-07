using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of legacy collections for the randomizer.</summary>
    public sealed class LegacyCollectionCreateHint : CreateHint
    {
        /// <summary>Supported types and the methods used to generate them.</summary>
        private static readonly (Type, Func<string[], RandomizerChainer, object>)[] s_Creators
            = new(Type, Func<string[], RandomizerChainer, object>)[]
            {
                (typeof(Hashtable), CreateDict<Hashtable>),
                (typeof(SortedList), CreateDict<SortedList>),
                (typeof(ListDictionary), CreateDict<ListDictionary>),
                (typeof(HybridDictionary), CreateDict<HybridDictionary>),
                (typeof(StringDictionary), CreateDict<StringDictionary>),
                (typeof(OrderedDictionary), CreateDict<OrderedDictionary>),
                (typeof(NameValueCollection), CreateDict<NameValueCollection>),

                (typeof(Array), (data, gen) => data),
                (typeof(Stack), (data, gen) => new Stack(data)),
                (typeof(Queue), (data, gen) => new Queue(data)),
                (typeof(ArrayList), (data, gen) => new ArrayList(data)),
                (typeof(BitArray), (data, gen) => new BitArray(data.Select(d => gen.Create<bool>()).ToArray())),

                (typeof(StringCollection), (data, gen) =>
                {
                    StringCollection result = new StringCollection();
                    foreach(string item in data)
                    {
                        result.Add(item);
                    }
                    return result;
                }),
            };

        /// <summary>Collections that the hint will create.</summary>
        internal static IEnumerable<Type> PotentialCollections => s_Creators.Select(i => i.Item1);

        /// <summary>Size details for created collections.</summary>
        private readonly int m_MinSize, m_Range;

        /// <summary>Specifies the size of generated collections.</summary>
        /// <param name="minSize">Min size for created collections.</param>
        /// <param name="range">Size variance for created collections.</param>
        public LegacyCollectionCreateHint(int minSize = 1, int range = 3)
        {
            m_MinSize = minSize;
            m_Range = range;
        }

        /// <summary>Tries to create a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>If the type could be created and the created instance.</returns>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            if (type.Inherits<IEnumerable>() && FindMatches(type).Any())
            {
                return (true, randomizer.Gen.NextItem(FindMatches(type)).Item2
                    .Invoke(CreateInternalData(randomizer), randomizer));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Finds potential collection matches for a type.</summary>
        /// <param name="type">Type to find matches for.</param>
        /// <returns>All possible matches.</returns>
        private static IEnumerable<(Type, Func<string[], RandomizerChainer, object>)> FindMatches(Type type)
        {
            return s_Creators.Where(m => type.IsInheritedBy(m.Item1));
        }

        /// <summary>Creates the type and populates it with data.</summary>
        /// <typeparam name="TDict">Type to create.</typeparam>
        /// <param name="keys">Keys to create in the type.</param>
        /// <param name="gen">Handles callback behavior for child values.</param>
        /// <returns>The created instance.</returns>
        private static TDict CreateDict<TDict>(string[] keys, RandomizerChainer gen)
        {
            dynamic data = Activator.CreateInstance<TDict>();
            for (int i = 0; i < keys.Length; i++)
            {
                data.Add(keys[i], gen.Create<string>());
            }
            return data;
        }

        /// <summary>Creates populated string array of data to use.</summary>
        /// <param name="randomizer">Callback to the randomizer to create child values.</param>
        /// <returns>Data populated with random values.</returns>
        private string[] CreateInternalData(RandomizerChainer randomizer)
        {
            string[] data = new string[m_MinSize + randomizer.Gen.Next(m_Range)];
            for (int i = 0; i < data.Length; i++)
            {
                data.SetValue(randomizer.Create<string>(), i);
            }
            return data;
        }
    }
}
