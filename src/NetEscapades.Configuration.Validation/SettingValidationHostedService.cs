#if NETSTANDARD2_1
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace NetEscapades.Configuration.Validation
{
    /// <summary>
    /// An <see cref="IHostedService"/> that validates <see cref="IValidatable"/> objects are valid on app startup
    /// </summary>
    public class SettingValidationHostedService : IHostedService
    {
        readonly IEnumerable<IValidatable> _validatableObjects;

        /// <summary>
        /// Create a new instance of <see cref="SettingValidationHostedService"/>
        /// </summary>
        /// <param name="validatableObjects"></param>
        public SettingValidationHostedService(IEnumerable<IValidatable> validatableObjects)
        {
            _validatableObjects = validatableObjects;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var validatableObject in _validatableObjects)
            {
                validatableObject.Validate();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
#endif