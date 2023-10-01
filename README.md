# ApacheConfigGenerator
This is the tool I've written to help me maintain Apache Virtual Hosts for multiple websites
# How to use
Clone the repository

Update config.json file with domain data as per example.

The script assumes that you have three environments your website is running on
- production - production environment
- devtest - QA development environment
- hotfix - QA environment used for urgent fixes.

with two types of websites
- regular - regular website listening on port 80 and 443 with ssl.
- redirection - vhosts that redirects to given url

You can modify these but be aware that this might require you to rewrite parts of the script.

We have three basic strategies for script generation:
- Regular configuration strategy holds basic configuration for vhost that listents both on port 80 and 433. Port 80 redirects to the 443.
- Redirection strategy creates a vhost that will redirect every request to the domain to the url specified by you - activate by adding ShouldRedirect: true and RedirectUrl to the Website object.
- Certificate generation strategy allows you to kickstart a vhost listening on both port 80 and 443 without the need to point to certificate file. This is not recommended for production as it will break your website. It might be used for Let's Encrypt HTTP challenge though DNS challenge is recommended. You can activate this mode by passing --generate-certs in command line.

Publish the script and run it from command line type 
```./ApacheConfigGenerator.exe```

The configuration files will be find in the root directory relative for the execution of the script inside of (ex. hotfix/vhosts).
