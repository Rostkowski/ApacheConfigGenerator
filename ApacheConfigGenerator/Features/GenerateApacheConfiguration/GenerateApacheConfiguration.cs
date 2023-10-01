using ApacheConfigGenerator.Enums;
using ApacheConfigGenerator.Interfaces;
using ApacheConfigGenerator.Models;
using ApacheConfigGenerator.Strategies;

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

        private static readonly List<Website> regularWebsites = new()
        {
            new Website("regular.rostkowski.uk", "offer-creator"),
        };

        private static readonly List<Website> redirections = new()
        {
            new Website("redirection.rostkowski.uk", "offer-creator", shouldRedirect: true, redirectUrl: "https://github.com/Rostkowski"),
        };

        private static readonly List<Website> landingPages = new()
        {
            new Website("landingpage.rostkowski.uk", "lp-DEMO", isLandingPage: true),
        };

        public static List<Website> websites = landingPages
            .Concat(regularWebsites)
            .Concat(redirections)
            .ToList();

        private IApacheConfigurationStrategy _strategy = new RegularConfigurationStrategy();

        public GenerateApacheConfiguration()
        {
        }

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

        public static void GetCommandToGenerateLetsEncryptCertificateForEachEnvironment(string path)
        {
            foreach (var environment in environments)
            {
                string letsEncryptCertificationCommand = $"sudo certbot certonly --cert-name {environment} -d ";

                foreach (var website in websites)
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

        public static void GetCommandToEnableVHostsForEachEnvironment(string path)
        {
            foreach (var environment in environments)
            {
                string a2ensiteCommand = string.Empty;

                foreach (var website in websites)
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
