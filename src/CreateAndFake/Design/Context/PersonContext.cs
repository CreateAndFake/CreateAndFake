using System;
using CreateAndFake.Design.Data;
using CreateAndFake.Design.Randomization;

namespace CreateAndFake.Design.Context
{
    /// <summary>Generates values for people.</summary>
    public sealed class PersonContext : BaseDataContext
    {
        /// <summary>Name fragment.</summary>
        private readonly Lazy<string> _firstName, _middleName, _lastName;

        /// <inheritdoc/>
        internal PersonContext(IRandom gen) : base(gen)
        {
            _firstName = new(() => Gen.NextItem(NameData.Values));
            _middleName = new(() => Gen.NextItem(NameData.Values));
            _lastName = new(() => Gen.NextItem(NameData.Values));
        }

        /// <summary>First name for the person.</summary>
        public string FirstName => _firstName.Value;

        /// <summary>Middle name for the person.</summary>
        public string MiddleName => _middleName.Value;

        /// <summary>Last name for the person.</summary>
        public string LastName => _lastName.Value;

        /// <summary>First and last name for the person.</summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>Initials for the name.</summary>
        public string Initials => $"{FirstName[0]}{MiddleName[0]}{LastName[0]}";
    }
}
