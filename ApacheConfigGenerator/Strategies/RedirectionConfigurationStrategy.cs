using ApacheConfigGenerator.Interfaces;
using ApacheConfigGenerator.Models;

namespace ApacheConfigGenerator.Strategies
{
    internal class RedirectionConfigurationStrategy : IApacheConfigurationStrategy
    {
        public string GenerateConfigurationFile(Website website, string environment)
        {
            return @$"<VirtualHost *:80>
    ServerName {website.WebsiteUrl}
    ServerAlias www.{website.WebsiteUrl}
            
    ServerSignature Off
    SSLEngine Off
    RewriteEngine On
    RewriteRule ^(.*)$ {website.RedirectUrl}
</VirtualHost>

<VirtualHost *:443>
    ServerName {website.WebsiteUrl}
    ServerAlias www.{website.WebsiteUrl}
            
    ServerSignature Off
    RewriteEngine On
    RewriteRule ^(.*)$ {website.RedirectUrl}
    SSLEngine On
    SSLCertificateFile /etc/letsencrypt/live/{environment}/fullchain.pem
    SSLCertificateKeyFile /etc/letsencrypt/live/{environment}/privkey.pem
    Include /etc/letsencrypt/options-ssl-apache.conf
</VirtualHost>";
        }
    }
}
