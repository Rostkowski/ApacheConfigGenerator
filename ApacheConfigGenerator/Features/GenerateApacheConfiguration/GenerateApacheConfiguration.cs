using ApacheConfigGenerator.Interfaces;
using ApacheConfigGenerator.Models;
using ApacheConfigGenerator.Strategies;

namespace ApacheConfigGenerator.Features.GenerateApacheConfiguration
{
    internal class GenerateApacheConfiguration
    {
        private IApacheConfigurationStrategy _strategy = new RegularConfigurationStrategy();

        public GenerateApacheConfiguration() { }

        public GenerateApacheConfiguration(IApacheConfigurationStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IApacheConfigurationStrategy strategy)
        {
            this._strategy = strategy;
        }

        public string GetConfigurationFileContent(Website website, string environment)
        {
            var websiteObject = (Website)website.ShallowCopy();

            if (environment != "production")
            {
                websiteObject.WebsiteUrl = string.Concat(environment, '.', websiteObject.WebsiteUrl);
            }
            Console.WriteLine($"Generating config file for {websiteObject.WebsiteUrl}");

            return _strategy.GenerateConfigurationFile(websiteObject, environment);
        }

        public string GetStrategyName()
        {
            return nameof(_strategy);
        }
    }
}
