using ApacheConfigGenerator.Enums;
using ApacheConfigGenerator.Features.GenerateApacheConfiguration;
using ApacheConfigGenerator.Strategies;

bool shouldUseCertGenerationMode = args.Length > 0 && args[0] == "--generate-certs";

var configGenerator = new GenerateApacheConfiguration();

string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;

if (shouldUseCertGenerationMode)
{
    configGenerator.SetStrategy(new CertificateGenerationConfigurationStrategy());
    GenerateApacheConfiguration.GetCommandToGenerateLetsEncryptCertificateForEachEnvironment(rootDirectory);
}

foreach (var environment in GenerateApacheConfiguration.environments)
{
    string environmentFolderPath = Path.Combine(rootDirectory, environment);

    if (!Directory.Exists(environmentFolderPath)) {
        Directory.CreateDirectory(environmentFolderPath);
    }

    string vhostsFolderPath = Path.Combine(environmentFolderPath, "vhosts");

    if(!Directory.Exists(vhostsFolderPath))
    {
        Directory.CreateDirectory(vhostsFolderPath);
    }

    foreach (var website in GenerateApacheConfiguration.websites)
    {
        if ((website.ShouldRedirect || website.IsLandingPage) && environment != nameof(WebsiteEnvironment.production)) continue;

        string configFileName = website.WebsiteUrl.Replace('.', '-');

        if (website.ShouldRedirect && !shouldUseCertGenerationMode)
        {
            configGenerator.SetStrategy(new RedirectionConfigurationStrategy());
        }
        else if (!shouldUseCertGenerationMode)
        {
            configGenerator.SetStrategy(new RegularConfigurationStrategy());
        }

        var config = configGenerator.GetConfigurationFileContent(website, environment);

        using var writer = new StreamWriter(Path.Combine(vhostsFolderPath, $"{environment}-{configFileName}.conf"));
        writer.Write(config);
    }
}