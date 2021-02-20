using System;
using System.Collections.Generic;
using System.Text;
using CreateAndFake.Design.Context;

namespace CreateAndFake.Design.Randomization
{
    /// <summary>For generating random predefined values.</summary>
    public sealed class DataRandom
    {
        /// <summary>Supported searchable property names for values.</summary>
        private static readonly Dictionary<string, Func<DataRandom, string>> _Matcher = new()
        {
            { "FIRSTNAME", gen => gen.Person.FirstName },
            { "MIDDLENAME", gen => gen.Person.MiddleName },
            { "LASTNAME", gen => gen.Person.LastName },
            { "FULLNAME", gen => gen.Person.FullName },
            { "INITIALS", gen => gen.Person.Initials }
        };

        /// <summary>All searchable names.</summary>
        internal static ICollection<string> SupportedProperties => _Matcher.Keys;

        /// <inheritdoc cref="IRandom"/>
        private readonly IRandom _gen;

        /// <inheritdoc cref="Person"/>
        private readonly Lazy<PersonContext> _person;

        /// <inheritdoc cref="PersonContext"/>
        public PersonContext Person => _person.Value;

        /// <summary>Sets up the data contexts.</summary>
        /// <param name="gen">Handles basic randomization.</param>
        internal DataRandom(IRandom gen)
        {
            _gen = gen ?? throw new ArgumentNullException(nameof(gen));
            _person = new(() => new PersonContext(_gen));
        }

        /// <summary>Finds a value for a property name.</summary>
        /// <param name="name">Name to find a value for.</param>
        /// <returns>Value if found; null otherwise.</returns>
        public string Find(string name)
        {
            if (_Matcher.TryGetValue(ToUpperOnly(name), out Func<DataRandom, string> finder))
            {
                return finder.Invoke(this);
            }
            else
            {
                return null;
            }
        }

        /// <summary>Converts the string to upper case letters only.</summary>
        /// <returns>The converted string.</returns>
        private static string ToUpperOnly(string value)
        {
            StringBuilder result = new();
            foreach (char c in value?.ToUpperInvariant() ?? "")
            {
                if (c is >= 'A' and <= 'Z')
                {
                    _ = result.Append(c);
                }
            }
            return result.ToString();
        }
    }
}
