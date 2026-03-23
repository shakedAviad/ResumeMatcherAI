namespace Backend.API.Configuration
{
    public sealed class StorageOptions
    {
        public const string SectionName = "Storage";

        public string ResumeJsonDirectory { get; set; } = string.Empty;
        public string ResumeFilesDirectory { get; set; } = string.Empty;
    }
}
