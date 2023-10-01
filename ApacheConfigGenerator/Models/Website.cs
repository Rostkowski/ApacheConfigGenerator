namespace ApacheConfigGenerator.Models
{
    internal sealed class Website
    {
        private const string LandingPagesFolderName = "LandingPages";
        public string WebsiteUrl { get; set; }
        public string FolderName { get; set; }
        public bool ShouldRedirect { get; set; }
        public string? RedirectUrl { get; set; }
        public bool IsLandingPage { get; set; }
        public bool IsSuspended { get; set; }

        public Website(string websiteUrl, string folderName, bool shouldRedirect = false, bool isLandingPage = false, string? redirectUrl = null)
        {
            if (shouldRedirect && string.IsNullOrWhiteSpace(redirectUrl))
            {
                throw new ArgumentException("RedirectUrl cannot be null if a website is redirecting");
            }

            WebsiteUrl = websiteUrl;
            FolderName = isLandingPage ? string.Concat(LandingPagesFolderName, '/', folderName) : folderName;
            ShouldRedirect = shouldRedirect;
            IsLandingPage = isLandingPage;
            RedirectUrl = redirectUrl;
        }

        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }
    }
}
