using ApacheConfigGenerator.Features.GenerateApacheConfiguration;
using ApacheConfigGenerator.Models;
using ApacheConfigGenerator.Strategies;

bool shouldUseCertGenerationMode = args.Length > 0 && args[0] == "--generate-certs";

List<string> environments = new()
{
    "production",
    "devtest",
    "hotfix"
};

List<Website> websites = new();

List<Website> regularWebsites = new()
{
    new Website("giganciprogramowania.edu.pl", "pl-PL"),
    new Website("codinggiants.com", "en-US"),
    new Website("codinggiants.co.uk", "en-GB"),
    new Website("codinggiants.si", "si-SI"),
    new Website("codinggiants.rs", "sr-RS"),
    new Website("codinggiants.me", "cnr-ME"),
    new Website("codinggiants.it", "it-IT"),
    new Website("codinggiants.hr", "hr-HR"),
    new Website("codinggiants.fr", "fr-FR"),
    new Website("codinggiants.es", "es-ES"),
    new Website("en.codinggiants.me", "en-ME"),
    new Website("codinggiants.de", "de-DE"),
    new Website("codinggiants.cz", "cs-CZ"),
    new Website("codinggiants.com.au", "en-AU"),
    new Website("codinggiants.cl", "es-CL"),
    new Website("codinggiants.bo", "es-BO"),
    new Website("codinggiants.bg", "bg-BG"),
    new Website("codinggiants.ba", "bs-BA"),
    new Website("al.codinggiants.me", "al-ME"),
    new Website("codinggiants.ae", "ar-AE"),
    new Website("codinggiants.ma", "ar-MA"),
    new Website("codinggiants.at", "de-AT"),
    new Website("codinggiants.ar", "es-AR"),
    new Website("codinggiants.sk", "sk-SK"),
    new Website("codinggiants.com.ve", "es-VE"),
    new Website("codinggiants.vn", "vi-VN"),
};

List<Website> redirections = new()
{
    new Website("codinggiants.us", "pl-PL", isRedirecting: true, redirectUrl: "https://codinggiants.com"),
    new Website("giganciedukacji.edu.pl", "pl-PL", isRedirecting: true, redirectUrl: "https://giganciprogramowania.edu.pl/kursy?active_type=MATH_ONLINE_8TH_GRADE"),
    new Website("codinggiants.sg", "pl-PL", isRedirecting: true, redirectUrl: "https://codinggiants.com"),
    new Website("konkurs.giganciprogramowania.edu.pl", "pl-PL", isRedirecting: true, redirectUrl: "https://hello.giganciprogramowania.edu.pl/akcja-aplikacja"),
};

List<Website> landingPages = new()
{
    new Website("gigancitravel.pl", "pl-PL-Travel", isLandingPage: true),
    new Website("talleres.codinggiants.cl", "es-CL-KZG", isLandingPage: true),
    new Website("workshop.codinggiants.co.uk", "en-GB-KZG", isLandingPage: true),
    new Website("kodujzgigantami.pl", "pl-PL-KZG", isLandingPage: true),
    new Website("talleres.codinggiants.ar", "es-AR-KZG", isLandingPage: true),
    new Website("talleres.codinggiants.mx", "es-MX-KZG", isLandingPage: true),
    new Website("talleres.codinggiants.es", "es-ES-KZG", isLandingPage: true),
    new Website("workshop.codinggiants.de", "de-DE-KZG", isLandingPage: true),
    new Website("workshops.codinggiants.at", "de-AT-KZG", isLandingPage: true),
    new Website("workshop.codinggiants.it", "it-IT-KZG", isLandingPage: true),
    new Website("ateliers.codinggiants.fr", "fr-FR-KZG", isLandingPage: true),
    new Website("workshops.codinggiants.si", "si-SI-KZG", isLandingPage: true),
    new Website("asociacionfp.es", "es-ES-Foundation", isLandingPage: true),
};

websites.AddRange(regularWebsites.Concat(redirections).Concat(landingPages));

var configGenerator = new GenerateApacheConfiguration();

if (shouldUseCertGenerationMode)
{
    configGenerator.SetStrategy(new CertificateGenerationConfigurationStrategy());
}

string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;

foreach (var environment in environments)
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

    foreach (var website in websites)
    {
        if ((website.IsRedirecting || website.IsLandingPage) && environment != "production") continue;

        string configFileName = website.WebsiteUrl.Replace('.', '-');

        if (website.IsRedirecting && !shouldUseCertGenerationMode)
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