using FluentAssertions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using Pss.ExampleBeers.Models.Model.Bars;
using Pss.ExampleBeers.MongoDB.Mongo;

namespace Pss.ExampleBeers.MongoDB.UnitTests.Mongo;

[TestFixture]
public class BarsCollectionTests
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
        BarsCollection.Name.Should().Be("bars");
    }
    
    [Test]
    public void Setup_RegistersClassMap()
    {
        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        BsonClassMap.IsClassMapRegistered(typeof(Bar)).Should().BeTrue();
    }
    
    [Test]
    public void Setup_IfCollectionDoesNotExist_CreatesCollection()
    {
        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        _mongoDatabaseMock.Verify(
            d => d.CreateCollection(BarsCollection.Name, It.IsAny<CreateCollectionOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void Setup_IfCollectionDoesExist_DoesNotCreateCollection()
    {
        var returnValue = new Mock<IAsyncCursor<string>>();
        returnValue.SetupSequence(v => v.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        returnValue.SetupGet(v => v.Current).Returns(new[] { BarsCollection.Name });

        _mongoDatabaseMock.Setup(d =>
                d.ListCollectionNames(It.IsAny<ListCollectionNamesOptions>(), It.IsAny<CancellationToken>()))
            .Returns(returnValue.Object);

        var sut = GetDefaultSut();
        sut.Setup(_mongoDatabaseMock.Object);

        _mongoDatabaseMock.Verify(
            d => d.CreateCollection(BarsCollection.Name, It.IsAny<CreateCollectionOptions>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }
    
    private static BarsCollection GetDefaultSut()
    {
        return new BarsCollection();
    }
}