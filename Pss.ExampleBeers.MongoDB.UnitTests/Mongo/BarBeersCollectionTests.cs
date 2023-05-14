using FluentAssertions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB.UnitTests.Mongo;

[TestFixture]
public class BarBeersCollectionTests
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
        BarBeersCollection.Name.Should().Be("barBeers");
    }
    
    [Test]
    public void Setup_RegistersClassMap()
    {
        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        BsonClassMap.IsClassMapRegistered(typeof(BarBeer)).Should().BeTrue();
    }
    
    [Test]
    public void Setup_IfCollectionDoesNotExist_CreatesCollection()
    {
        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        _mongoDatabaseMock.Verify(
            d => d.CreateCollection(BarBeersCollection.Name, It.IsAny<CreateCollectionOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Setup_IfCollectionDoesExist_DoesNotCreateCollection()
    {
        var returnValue = new Mock<IAsyncCursor<string>>();
        returnValue.SetupSequence(v => v.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        returnValue.SetupGet(v => v.Current).Returns(new[] { BarBeersCollection.Name });

        _mongoDatabaseMock.Setup(d =>
                d.ListCollectionNames(It.IsAny<ListCollectionNamesOptions>(), It.IsAny<CancellationToken>()))
            .Returns(returnValue.Object);

        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        _mongoDatabaseMock.Verify(
            d => d.CreateCollection(BarBeersCollection.Name, It.IsAny<CreateCollectionOptions>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test]
    public void Setup_SetsUpIndex()
    {
        var mockIndexManager = new Mock<IMongoIndexManager<BarBeer>>();
        var mockCollection = new Mock<IMongoCollection<BarBeer>>();
        mockCollection.SetupGet(c => c.Indexes).Returns(mockIndexManager.Object);
        _mongoDatabaseMock
            .Setup(d => d.GetCollection<BarBeer>(BarBeersCollection.Name, It.IsAny<MongoCollectionSettings>()))
            .Returns(mockCollection.Object);

        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        mockIndexManager.Verify(
            i => i.CreateOne(It.IsAny<CreateIndexModel<BarBeer>>(), It.IsAny<CreateOneIndexOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    private static BarBeersCollection GetDefaultSut()
    {
        return new BarBeersCollection();
    }
}