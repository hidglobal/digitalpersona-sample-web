using System.Runtime.Serialization;

namespace DPWebDemo.Services.Biometric
{
    /// <summary>
    /// Details the purpose of this biometric sample.
    /// If fingerprint image is sent as biometric sample, ANY purpose should be set.
    /// </summary>
    [DataContract]
    public enum BiometricSamplePurpose
    {
        /// <summary>
        /// Any purpose.
        /// </summary>
        [EnumMember]
        Any = 0,

        /// <summary>
        /// Purpose is verification.
        /// </summary>
        [EnumMember]
        Verify = 1,

        /// <summary>
        /// Purpose is identification.
        /// </summary>
        [EnumMember]
        Identify = 2,

        /// <summary>
        /// Purpose is enrollment.
        /// </summary>
        [EnumMember]
        Enroll = 3,

        /// <summary>
        /// Purpose is enrollment for verification.
        /// </summary>
        [EnumMember]
        EnrollForVerificationOnly = 4,

        /// <summary>
        /// Purpose is enrollment for identification.
        /// </summary>
        [EnumMember]
        EnrollForIdentificationOnly = 5,

        /// <summary>
        /// Purpose is audit.
        /// </summary>
        [EnumMember]
        Audit = 6,

    }
}
