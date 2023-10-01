namespace ApacheConfigGenerator.Models
{
    internal sealed class Website
    {
        private const string LandingPagesFolderName = "LandingPages";
        public string WebsiteUrl { get; set; }
        public string FolderName { get; set; }
        public bool IsRedirecting { get; set; }
        public string? RedirectUrl { get; set; }
        public bool IsLandingPage { get; set; }
        public bool IsSuspended { get; set; }

        public Website(string websiteUrl, string folderName, bool isRedirecting = false, bool isLandingPage = false, string? redirectUrl = null)
        {
            WebsiteUrl = websiteUrl;
            FolderName = isLandingPage ? string.Concat(LandingPagesFolderName, '/', folderName) : folderName;
            IsRedirecting = isRedirecting;
            IsLandingPage = isLandingPage;
            RedirectUrl = redirectUrl;

            if (isRedirecting && string.IsNullOrWhiteSpace(RedirectUrl) )
            {
                throw new ArgumentException("RedirectUrl cannot be null if a website is redirecting");
            }
        }

        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }
    }
}
