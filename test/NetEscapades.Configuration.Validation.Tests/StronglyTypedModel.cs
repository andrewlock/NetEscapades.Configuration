using System;

namespace NetEscapades.Configuration.Validation.Tests
{
    public class StronglyTypedModel : IValidatable
    {
        public string Url { get; set; }
        public int Port { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                throw new SettingsValidationException(nameof(StronglyTypedModel), nameof(Url), "Must not be null or whitespace");
            }

            // will throw if invalid Url
            var uri = new Uri(Url);

            if(Port <= 0)
            {
                throw new SettingsValidationException(nameof(StronglyTypedModel), nameof(Port), "Must be greater than 0");
            }
        }
    }
}
