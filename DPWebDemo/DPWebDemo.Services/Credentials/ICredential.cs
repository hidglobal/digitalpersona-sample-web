namespace DPWebDemo.Services
{
    /// <summary>
    /// Common credential interface.
    /// </summary>
    public interface ICredential
    {
        /// <summary>
        /// Credential type.
        /// </summary>
        CredentialType Type { get; }

        /// <summary>
        /// Credential data object.
        /// </summary>
        object Data { get; }
    }
}