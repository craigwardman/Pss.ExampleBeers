using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB.UnitTests.Mongo
{
    [TestFixture]
    public class MongoConnectionTests
    {
        [SetUp]
        public void SetUp()
        {
            _config.ConnectionString = "mongodb://test";
            _config.DatabaseName = "test-db";

            _stubBootstrapItems = new List<IBootstrapped>();
        }

        private readonly MongoDataStoreConfig _config = new();
        private List<IBootstrapped> _stubBootstrapItems = new();

        [Test]
        public void Ctor_ConfigNull_Throws()
        {
            var act = () => new MongoConnection(null!, Array.Empty<IBootstrapped>());

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("config");
        }

        [Test]
        public void Ctor_BootstrapsAllChildItems()
        {
            _stubBootstrapItems = new List<IBootstrapped>
            {
                new Mock<IBootstrapped>().Object,
                new Mock<IBootstrapped>().Object,
                new Mock<IBootstrapped>().Object
            };

            GetDefaultSut();

            using (new AssertionScope())
            {
                _stubBootstrapItems.ForEach(i =>
                    Mock.Get(i).Verify(e => e.Setup(It.IsAny<IMongoDatabase>()), Times.Once));
            }
        }

        [Test]
        public void Database_IsConfiguredCorrectly()
        {
            var sut = GetDefaultSut();
            var result = sut.Database;

            using (new AssertionScope())
            {
                result.Client.Settings.Server.Host.Should().Be("test");
                result.DatabaseNamespace.DatabaseName.Should().Be("test-db");
            }
        }

        private MongoConnection GetDefaultSut()
        {
            return new MongoConnection(new OptionsWrapper<MongoDataStoreConfig>(_config), _stubBootstrapItems);
        }
    }
}