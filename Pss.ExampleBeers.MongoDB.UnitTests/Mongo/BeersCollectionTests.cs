using FluentAssertions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using Pss.ExampleBeers.Models.Model.Beers;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB.UnitTests.Mongo;

[TestFixture]
public class BeersCollectionTests
{
    [SetUp]
    public void SetUp()
    {
        _mongoDatabaseMock.Reset();
    }

    private readonly Mock<IMongoDatabase> _mongoDatabaseMock = new()
    {
        DefaultValue = DefaultValue.Mock
    };
    
    [Test]
    public void Name_Is_Correct()
    {
        BeersCollection.Name.Should().Be("beers");
    }
    
    [Test]
    public void Setup_RegistersClassMap()
    {
        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        BsonClassMap.IsClassMapRegistered(typeof(Beer)).Should().BeTrue();
    }
    
    [Test]
    public void Setup_IfCollectionDoesNotExist_CreatesCollection()
    {
        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        _mongoDatabaseMock.Verify(
            d => d.CreateCollection(BeersCollection.Name, It.IsAny<CreateCollectionOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Setup_IfCollectionDoesExist_DoesNotCreateCollection()
    {
        var returnValue = new Mock<IAsyncCursor<string>>();
        returnValue.SetupSequence(v => v.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        returnValue.SetupGet(v => v.Current).Returns(new[] { BeersCollection.Name });

        _mongoDatabaseMock.Setup(d =>
                d.ListCollectionNames(It.IsAny<ListCollectionNamesOptions>(), It.IsAny<CancellationToken>()))
            .Returns(returnValue.Object);

        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        _mongoDatabaseMock.Verify(
            d => d.CreateCollection(BeersCollection.Name, It.IsAny<CreateCollectionOptions>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test]
    public void Setup_SetsUpIndex()
    {
        var mockIndexManager = new Mock<IMongoIndexManager<Beer>>();
        var mockCollection = new Mock<IMongoCollection<Beer>>();
        mockCollection.SetupGet(c => c.Indexes).Returns(mockIndexManager.Object);
        _mongoDatabaseMock
            .Setup(d => d.GetCollection<Beer>(BeersCollection.Name, It.IsAny<MongoCollectionSettings>()))
            .Returns(mockCollection.Object);

        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        mockIndexManager.Verify(
            i => i.CreateOne(It.IsAny<CreateIndexModel<Beer>>(), It.IsAny<CreateOneIndexOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    private static BeersCollection GetDefaultSut()
    {
        return new BeersCollection();
    }
}