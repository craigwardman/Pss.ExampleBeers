using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Pss.ExampleBeers.ApplicationServices.UnitTests
{
    [TestFixture]
    public class ServiceCollectionExtensionTests
    {
        [Test]
        public void AddDecisionDataStoreProvider_WhenCalled_ConfiguresValidContainer()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddApplicationServices();

            serviceCollection.Invoking(sc => sc.BuildServiceProvider(true)).Should().NotThrow();
        }
    }
}