using MongoDB.Driver;

namespace Pss.ExampleBeers.MongoDB.Mongo
{
    internal interface IMongoConnection
    {
        IMongoDatabase Database { get; }
    }
}