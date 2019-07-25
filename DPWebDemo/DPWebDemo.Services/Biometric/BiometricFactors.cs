using System;
using System.Runtime.Serialization;

namespace DPWebDemo.Services.Biometric
{
    /// <summary>
    /// Biometric featurie (fingerprint, voice).
    /// </summary>
    [DataContract]
    [Flags]
    public enum BiometricFactors
    {
        /// <summary>
        /// Multiply featuries.
        /// </summary>
        [EnumMember]
        Multiple = 1,

        /// <summary>
        /// Face.
        /// </summary>
        [EnumMember]
        Face = 2,

        /// <summary>
        /// Voice.
        /// </summary>
        [EnumMember]
        Voice = 4,

        /// <summary>
        /// Fingerprint.
        /// </summary>
        [EnumMember]
        Fingerprint = 8,

        /// <summary>
        /// Iris (eye).
        /// </summary>
        [EnumMember]
        Iris = 16,

        /// <summary>
        /// Retina (eye).
        /// </summary>
        [EnumMember]
        Retina = 32,

        /// <summary>
        /// Hand geometry.
        /// </summary>
        [EnumMember]
        HandGeometry = 64,

        /// <summary>
        /// Signature dynamics.
        /// </summary>
        [EnumMember]
        SignatureDynamics = 128,

        /// <summary>
        /// Keystroke dynamics.
        /// </summary>
        [EnumMember]
        KeystrokeDynamics = 256,

        /// <summary>
        /// Lip Movement.
        /// </summary>
        [EnumMember]
        LipMovement = 512,

        /// <summary>
        /// Thermal Face Image.
        /// </summary>
        [EnumMember]
        ThermalFaceImage = 1024,

        /// <summary>
        /// Thermal Hand Image.
        /// </summary>
        [EnumMember]
        ThermalHandImage = 2048,

        /// <summary>
        /// Gait.
        /// </summary>
        [EnumMember]
        Gait = 4096
    }
}
