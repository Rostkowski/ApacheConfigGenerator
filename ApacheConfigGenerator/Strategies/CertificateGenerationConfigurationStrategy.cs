using ApacheConfigGenerator.Interfaces;
using ApacheConfigGenerator.Models;

namespace ApacheConfigGenerator.Strategies
{
    internal class CertificateGenerationConfigurationStrategy : IApacheConfigurationStrategy
    {
        public string GenerateConfigurationFile(Website website, string environment)
        {
            return @$"<VirtualHost *:80>
    ServerName {website.WebsiteUrl}
    ServerAlias www.{website.WebsiteUrl}
            
    ServerSignature Off
    SSLEngine Off
    DocumentRoot /var/www/html/{website.DirectoryPath}
</VirtualHost>

<VirtualHost *:443>
    ServerName {website.WebsiteUrl}
    ServerAlias www.{website.WebsiteUrl}
            
    ServerSignature Off
    DocumentRoot /var/www/html/{website.DirectoryPath}
    SSLEngine Off
</VirtualHost>";
        }
    }
}
