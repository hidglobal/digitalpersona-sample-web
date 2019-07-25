using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Credential type (Password,Pin, etc.)
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CredentialType
    {
        /// <summary>
        /// Password.
        /// </summary>
        [EnumMember(Value = "D1A1F561-E14A-4699-9138-2EB523E132CC")]
        Password,

        /// <summary>
        /// PIN (Personal Identification Number).
        /// </summary>
        [EnumMember(Value = "8A6FCEC3-3C8A-40C2-8AC0-A039EC01BA05")]
        Pin,

        /// <summary>
        /// Fingerprint.
        /// </summary>
        [EnumMember(Value = "AC184A13-60AB-40e5-A514-E10F777EC2F9")]
        Fingerprint,

        /// <summary>
        /// Bluetooth device.
        /// </summary>
        [EnumMember(Value = "E750A180-577B-47F7-ACD9-F89A7E27FA49")]
        Bluetooth,

        /// <summary>
        /// Live Questions.
        /// </summary>
        [EnumMember(Value = "B49E99C6-6C94-42DE-ACD7-FD6B415DF503")]
        LiveQuestions,

        /// <summary>
        /// One time password.
        /// </summary>
        [EnumMember(Value = "324C38BD-0B51-4E4D-BD75-200DA0C8177F")]
        OneTimePassword,

        /// <summary>
        /// Proximity/contactless card.
        /// </summary>
        [EnumMember(Value = "F674862D-AC70-48CA-B73E-64A22F3BAC44")]
        ProximityContactlessCard,

        /// <summary>
        /// Proximity card.
        /// </summary>
        [EnumMember(Value = "1F31360C-81C0-4EE0-9ACD-5A4400F66CC2")]
        ProximityCard,

        /// <summary>
        /// Samrtcard.
        /// </summary>
        [EnumMember(Value = "D66CC98D-4153-4987-8EBE-FB46E848EA98")]
        Smartcard,

        /// <summary>
        /// Contactless card.
        /// </summary>
        [EnumMember(Value = "7BF3E290-5BA5-4C2D-AA33-24B48C189399")]
        ContactlessCard

    }
}
