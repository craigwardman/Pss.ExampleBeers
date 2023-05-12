using System.Text.RegularExpressions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Pss.ExampleBeers.IntegrationTests.ValueRetrievers;

[Binding]
public class PercentageTransformations : IValueRetriever
{
    private static readonly Regex Matcher = new("(\\d+)%");
    
    [StepArgumentTransformation]
    public double PercentStringToDouble(string currencyString)
    {
        var match = Matcher.Match(currencyString);
        return match.Success ? double.Parse(match.Groups[1].Value) : 0;
    }

    public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
    {
        return propertyType == typeof(double) && Matcher.Match(keyValuePair.Value).Success;
    }

    public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
    {
        return PercentStringToDouble(keyValuePair.Value);
    }
}