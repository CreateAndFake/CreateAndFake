using System;
using System.Collections.Generic;
using System.Text;
using CreateAndFake.Design.Context;

namespace CreateAndFake.Design.Randomization;

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

    /// <summary>Initializes a new instance of the <see cref="DataRandom"/> class.</summary>
    /// <param name="gen">Manages the randomization process.</param>
    internal DataRandom(IRandom gen)
    {
        _gen = gen ?? throw new ArgumentNullException(nameof(gen));
        _person = new(() => new PersonContext(_gen));
    }

    /// <summary>Finds a value for the identifying <paramref name="name"/>.</summary>
    /// <param name="name">Name to find a value for.</param>
    /// <returns>The value if found; <c>null</c> otherwise.</returns>
    public string Find(string name)
    {
        return _Matcher.TryGetValue(ToUpperOnly(name), out Func<DataRandom, string> finder)
            ? finder.Invoke(this)
            : null;
    }

    /// <summary>Converts <paramref name="value"/> to upper case letters only.</summary>
    /// <param name="value">String to convert.</param>
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
