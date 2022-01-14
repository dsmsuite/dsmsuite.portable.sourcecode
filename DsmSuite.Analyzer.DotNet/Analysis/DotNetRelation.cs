namespace DsmSuite.Analyzer.DotNet.Analysis
{
    public class DotNetRelation
    {
        public DotNetRelation(string consumerName, string providerName, string type)
        {
            ConsumerName = consumerName;
            ProviderName = providerName;
            Type = type;
        }

        public string ConsumerName { get; }
        public string ProviderName { get; }
        public string Type { get; }
    }
}
