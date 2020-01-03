using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace WebDemoProject3_0
{
    public class TestSettings: IValidatable
    {
        public void Validate()
        {
            // Uncomment this line to throw on startup
            // throw new ValidationException("Throwing test exception");
        }
    }
}
