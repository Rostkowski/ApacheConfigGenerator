namespace ApacheConfigGenerator.Models
{
    internal sealed class Website
    {
        public string WebsiteUrl { get; set; }
        public string DirectoryPath { get; set; }
        public bool ShouldRedirect { get; set; }
        public string? RedirectUrl { get; set; }
        public bool IsLandingPage { get; set; }

        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }
    }
}
