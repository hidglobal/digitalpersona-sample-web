using System.Runtime.Serialization;

namespace DPWebDemo.Services.Biometric
{
    /// <summary>
    /// BioSampleFormat describes vendor specific format of Biometric sample.
    /// </summary>
    [DataContract]
    public class BiometricSampleFormat : DataObject
    {
        /// <summary>
        /// Biometric owner ID registered with IBIA (http://www.ibia.org/base/cbeff/_biometric_org.phpx).
        /// 51 for DigitalPersona, 49 for Neurotechnologija
        /// </summary>
        [DataMember(Name = "FormatOwner")]
        public BiometricFormatOwner FormatOwner { get; private set; }

        /// <summary>
        /// Value is assigned by the Format Owner and may optionally be registered by the IBIA.
        /// We don’t support any FormatID in this version so it should be set to 0.
        /// </summary>
        [DataMember(Name = "FormatID")]
        public int FormatId { get; private set; }

        /// <summary>
        /// Initialize new instance of <see cref="BiometricSampleFormat"/>.
        /// </summary>
        internal BiometricSampleFormat()
        {
            FormatId = 0;
        }

        /// <summary>
        /// Initialize new instance of <see cref="BiometricSampleFormat"/> with specific format owner.
        /// </summary>
        /// <param name="formatOwner">Biometric format owner.</param>
        internal BiometricSampleFormat(BiometricFormatOwner formatOwner)
            : this()
        {
            FormatOwner = formatOwner;
        }
    }
}
