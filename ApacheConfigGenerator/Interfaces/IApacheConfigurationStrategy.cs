using ApacheConfigGenerator.Models;

namespace ApacheConfigGenerator.Interfaces
{
    internal interface IApacheConfigurationStrategy
    {
        string GenerateConfigurationFile(Website website, string environment);
    }
}
