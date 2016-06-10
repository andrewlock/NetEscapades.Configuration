namespace NetEscapades.Extensions.Configuration.Yaml
{
    public static class Resources
    {
        public const string Error_KeyIsDuplicated = "A duplicate key '{0}' was found.";
        public const string Error_FileNotFound = "The configuration file '{0}' was not found and is not optional.";
        public const string Error_InvalidFilePath = "File path must be a non-empty string.";
        public const string Error_YamlParseError = "Could not parse the YAML file: {0}";
    }
}