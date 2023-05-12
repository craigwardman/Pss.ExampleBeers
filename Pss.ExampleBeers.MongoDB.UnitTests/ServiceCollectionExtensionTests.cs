using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pss.ExampleBeers.MongoDB.UnitTests
{
    [TestFixture]
    public class ServiceCollectionExtensionTests
    {
        [Test]
        public void AddDecisionDataStoreProvider_WhenCalled_ConfiguresValidContainer()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMongoRepository(new ConfigurationBuilder().Build());

            serviceCollection.Invoking(sc => sc.BuildServiceProvider(true)).Should().NotThrow();
        }
    }
}