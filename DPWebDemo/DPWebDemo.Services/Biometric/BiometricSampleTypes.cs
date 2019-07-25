using System;
using System.Runtime.Serialization;

namespace DPWebDemo.Services.Biometric
{
    /// <summary>
    /// BioSampleType enumeration details type of Biometric sample.
    /// </summary>
    [DataContract]
    [Flags]
    public enum BiometricSampleTypes
    {
        /// <summary>
        /// Means raw (unprocessed) biometric sample is presented.
        /// In case of Fingerprint this means Fingerprint Image is presented.
        /// </summary>
        [EnumMember]
        Raw = 0x01,

        /// <summary>
        /// Means partially processed biometric sample is presented. 
        /// In case of Fingerprint this means Fingerprint Feature Set is presented.
        /// </summary>
        [EnumMember]
        Intermediate = 0x02,

        /// <summary>
        /// Means fully processed biometric sample is presented. 
        /// In case of Fingerprint this means Fingerprint Template is presented.
        /// </summary>
        [EnumMember]
        Processed = 0x04,

        /// <summary>
        /// Encrypted data.
        /// Not supported in this version.
        /// </summary>
        [EnumMember]
        Encrypted = 0x10,

        /// <summary>
        /// Singned data.
        /// Not supported in this version.
        /// </summary>
        [EnumMember]
        Signed = 0x20,

    }
}
