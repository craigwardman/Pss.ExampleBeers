using MongoDB.Driver;

namespace Pss.ExampleBeers.MongoDB.Mongo
{
    internal interface IBootstrapped
    {
        void Setup(IMongoDatabase database);
    }
}