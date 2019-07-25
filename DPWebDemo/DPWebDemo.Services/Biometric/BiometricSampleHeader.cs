using System;
using System.Runtime.Serialization;

namespace DPWebDemo.Services.Biometric
{
    /// <summary>
    /// Detailed information about Biometric sample.
    /// </summary>
    [DataContract]
    public class BiometricSampleHeader
    {
        /// <summary>
        /// Biometric factor. Must be set to 8 for fingerprint.
        /// </summary>
        [DataMember]
        public BiometricFactors Factor { get; private set; }

        /// <summary>
        /// Format owner (vendor) information.
        /// </summary>
        [DataMember]
        public BiometricSampleFormat Format { get; private set; }

        /// <summary>
        /// type of biometric sample.
        /// </summary>
        [DataMember]
        public BiometricSampleTypes Type { get; private set; }

        /// <summary>
        /// Purpose of biometric sample.
        /// </summary>
        [DataMember]
        public BiometricSamplePurpose Purpose { get; private set; }

        /// <summary>
        /// Quality of biometric sample. If we don't support quality it should be set to -1.
        /// </summary>
        [DataMember]
        public sbyte Quality { get; private set; }

        /// <summary>
        /// Encryption of biometric sample.
        /// </summary>
        [DataMember]
        public BiometricSampleEncryption Encryption { get; private set; }

        /// <summary>
        /// Private consructor to prevent creating instance.
        /// </summary>
        private BiometricSampleHeader()
        {
        }

        /// <summary>
        /// Get predefined <see cref="BiometricSampleHeader"/> by <see cref="BiometricSampleHeaderType"/>.
        /// </summary>
        /// <param name="headerType">Header type.</param>
        /// <returns>Corresponding <see cref="BiometricSampleHeader"/>.</returns>
        public static BiometricSampleHeader GetHeader(BiometricSampleHeaderType headerType)
        {
            switch (headerType)
            {
                case BiometricSampleHeaderType.DigitalPersonaFingerprintFeatureSet:
                    return new BiometricSampleHeader
                    {
                        Factor = BiometricFactors.Fingerprint,
                        Format = new BiometricSampleFormat(BiometricFormatOwner.DigitalPersona),
                        Type = BiometricSampleTypes.Intermediate,
                        Purpose = BiometricSamplePurpose.Any,
                        Quality = -1,
                        Encryption = BiometricSampleEncryption.None
                    };
                case BiometricSampleHeaderType.DigitalPersonaFingerprintImage:
                    return new BiometricSampleHeader
                    {
                        Factor = BiometricFactors.Fingerprint,
                        Format = new BiometricSampleFormat(BiometricFormatOwner.DigitalPersona),
                        Type = BiometricSampleTypes.Raw,
                        Purpose = BiometricSamplePurpose.Any,
                        Quality = -1,
                        Encryption = BiometricSampleEncryption.None
                    };
                default:
                    throw new NotSupportedException(headerType + " does not support.");
            }
        }
    }
}
