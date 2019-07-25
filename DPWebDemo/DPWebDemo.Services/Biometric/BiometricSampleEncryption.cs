using System.Runtime.Serialization;

namespace DPWebDemo.Services.Biometric
{
    /// <summary>
    /// Details encryption algorithm been used to protect biometric sample.
    /// We strongly recommend use XTEA encryption for biometric samples (as we do in Pro and Altus product)
    /// but we recognize complexity of XTEA implementation on different platforms such as Android so we will accept unencrypted sample as well.
    /// </summary>
    [DataContract]
    public enum BiometricSampleEncryption
    {
        /// <summary>
        /// Data is not encrypted.
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// Biometric sample provided is encrypted using DigitalPersona implementation of XTEA with hardcoded key.
        /// </summary>
        [EnumMember]
        Xtea = 1,
    }
}
