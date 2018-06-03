namespace NetEscapades.Configuration.Validation
{
    /// <summary>
    /// An interface that can be implemented to ensure an object (typically settings)
    /// is configured correctly, before starting the application
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// Called during application startup to ensure the object is configured correctly.
        /// Should throw an exception if invalid
        /// </summary>
        void Validate();
    }
}
