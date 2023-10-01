using ApacheConfigGenerator.Enums;
using ApacheConfigGenerator.Interfaces;
using ApacheConfigGenerator.Models;
using ApacheConfigGenerator.Strategies;
using System.Text.Json;

namespace ApacheConfigGenerator.Features.GenerateApacheConfiguration
{
    internal class GenerateApacheConfiguration
    {

        public static readonly List<string> environments = new()
        {
            nameof(WebsiteEnvironment.production),
            nameof(WebsiteEnvironment.devtest),
            nameof(WebsiteEnvironment.hotfix)
        };

        public List<Website> Websites { get; set; }

        private IApacheConfigurationStrategy _strategy = new RegularConfigurationStrategy();

        public GenerateApacheConfiguration()
        {
            using StreamReader reader = new("config.json");
            Websites = JsonSerializer.Deserialize<List<Website>>(reader?.ReadToEnd());
        }

        public GenerateApacheConfiguration(IApacheConfigurationStrategy strategy)
        {
            using StreamReader reader = new("config.json");
            Websites = JsonSerializer.Deserialize<List<Website>>(reader?.ReadToEnd());
            _strategy = strategy;
        }

        public void SetStrategy(IApacheConfigurationStrategy strategy)
        {
            this._strategy = strategy;
        }

        public string GetConfigurationFileContent(Website website, string environment)
        {
            var websiteObject = GetWebsiteObjectForCurrentWebsiteEnvironment(website, environment);
            Console.WriteLine($"Generating config file for {websiteObject.WebsiteUrl}");

            return _strategy.GenerateConfigurationFile(websiteObject, environment);
        }

        private static string GetWebsiteUrl(Website website, string environment)
        {
            return GetWebsiteObjectForCurrentWebsiteEnvironment(website, environment).WebsiteUrl;
        }

        private static Website GetWebsiteObjectForCurrentWebsiteEnvironment(Website website, string environment)
        {
            var websiteObject = (Website)website.ShallowCopy();

            if (environment != nameof(WebsiteEnvironment.production))
            {
                websiteObject.WebsiteUrl = string.Concat(environment, '.', websiteObject.WebsiteUrl);
            }

            return websiteObject;
        }

        public void GetCommandToGenerateLetsEncryptCertificateForEachEnvironment(string path)
        {
            foreach (var environment in environments)
            {
                string letsEncryptCertificationCommand = $"sudo certbot certonly --cert-name {environment} -d ";

                foreach (var website in this.Websites)
                {
                    var websiteUrl = GetWebsiteUrl(website, environment);

                    letsEncryptCertificationCommand += $"{websiteUrl},";
                    if (environment == nameof(WebsiteEnvironment.production))
                    {
                        letsEncryptCertificationCommand += $"www.{websiteUrl},";
                    }
                }

                using var writer = new StreamWriter(Path.Combine(path, $"lets_encrypt_{environment}.txt"));
                writer.Write(letsEncryptCertificationCommand[..^1]);
            }
        }

        public void GetCommandToEnableVHostsForEachEnvironment(string path)
        {
            foreach (var environment in environments)
            {
                string a2ensiteCommand = string.Empty;

                foreach (var website in this.Websites)
                {
                    string configFileName = website.WebsiteUrl.Replace('.', '-');
                    var websiteUrl = GetWebsiteUrl(website, environment);

                    a2ensiteCommand += $"sudo a2ensite {environment}-{configFileName}.conf \n";
                }

                using var writer = new StreamWriter(Path.Combine(path, $"a2ensite_{environment}.txt"));
                writer.Write(a2ensiteCommand);
            }
        }

        public string GetStrategyName()
        {
            return nameof(_strategy);
        }
    }
}
