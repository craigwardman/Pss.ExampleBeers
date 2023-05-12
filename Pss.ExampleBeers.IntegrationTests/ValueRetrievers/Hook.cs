using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Pss.ExampleBeers.IntegrationTests.ValueRetrievers;

[Binding]
public static class Hook
{
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Service.Instance.ValueRetrievers.Register(new PercentageTransformations());
    }
}