using System;
using System.ComponentModel.DataAnnotations;

namespace NetEscapades.Configuration.Validation.Tests
{
    public class ValidateObjectModel : IValidatable
    {
        [Required(AllowEmptyStrings = false), Url]
        public string Url { get; set; }

        [Range(minimum: 1, maximum: 65535)]
        public int Port { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }
}
