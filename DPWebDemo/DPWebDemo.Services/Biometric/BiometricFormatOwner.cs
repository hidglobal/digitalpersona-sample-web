using System.Runtime.Serialization;

namespace DPWebDemo.Services.Biometric
{
    /// <summary>
    /// Biometric owner ID registered with IBIA (http://www.ibia.org/base/cbeff/_biometric_org.phpx).
    /// 51 for DigitalPersona, 49 for Neurotechnologija
    /// </summary>
    [DataContract]
    public enum BiometricFormatOwner
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Neurotechnology fromat.
        /// </summary>
        [EnumMember]
        NeuroTechnology = 49,

        /// <summary>
        /// DigitalPersona, Inc. format.
        /// </summary>
        [EnumMember]
        DigitalPersona = 51

    }
}
