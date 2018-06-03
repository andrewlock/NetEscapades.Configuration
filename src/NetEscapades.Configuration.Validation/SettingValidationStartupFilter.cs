using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace NetEscapades.Configuration.Validation
{
    /// <summary>
    /// An <see cref="IStartupFilter"/> that validates <see cref="IValidatable"/> objects are valid on app startup
    /// </summary>
    public class SettingValidationStartupFilter : IStartupFilter
    {
        readonly IEnumerable<IValidatable> _validatableObjects;

        /// <summary>
        /// Create a new instance of <see cref="SettingValidationStartupFilter"/>
        /// </summary>
        /// <param name="validatableObjects"></param>
        public SettingValidationStartupFilter(IEnumerable<IValidatable> validatableObjects)
        {
            _validatableObjects = validatableObjects;
        }

        /// <inheritdoc />
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            foreach (var validatableObject in _validatableObjects)
            {
                validatableObject.Validate();
            }

            //don't alter the configuration
            return next;
        }
    }
}