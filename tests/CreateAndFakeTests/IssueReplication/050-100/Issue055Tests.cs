using System.Collections.Generic;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue055Tests
    {
        public class User
        {
            public string Street { get; set; }

            public int ZipCode { get; set; }

            public string MailingAddress => Street + ZipCode;
        }

        public class Details
        {
            public string Name { get; set; }
            public string Year { get; set; }

            public IEnumerable<string> Directors { get; set; }
        }

        public interface IStorage
        {
            IEnumerable<Details> Find(string name);
        }

        internal class Endpoint
        {
            private readonly IStorage _db;
            public Endpoint(IStorage db)
            {
                _db = db;
            }

            public IEnumerable<string> GetDirectors(string name, string year)
            {
                return _db.Find(name).FirstOrDefault(m => m.Year == year).Directors;
            }
        }

        [Theory, RandomData]
        internal static void Issue055_TestGetMovieDirectors([Fake] IStorage db, Endpoint api, [Size(2)] Details[] movies)
        {
            db.Find(movies[0].Name).SetupReturn(movies, Times.Once);
            api.GetDirectors(movies[0].Name, movies[0].Year).Assert().Is(movies[0].Directors);
            db.VerifyAllCalls();
        }

        [Fact]
        internal static void Issue055_TestMailingAddressUpdates()
        {
            User details = Tools.Randomizer.Create<User>();
            User copy = Tools.Duplicator.Copy(details);

            details.ZipCode = Tools.Mutator.Variant(details.ZipCode);

            Tools.Asserter.IsNot(copy.MailingAddress, details.MailingAddress);
        }
    }
}
