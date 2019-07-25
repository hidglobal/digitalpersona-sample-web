using System.Runtime.Serialization;

namespace DPWebDemo.Services
{
    /// <summary>
    /// UserName name format.
    /// </summary>
    [DataContract]
    public enum UserNameFormat : ushort
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Down-Level Logon Name (SAM) format (DOMAIN\UserName).
        /// </summary>
        /// <remarks>
        /// Windows NT® 4.0 account name. The domain-only version includes trailing backslashes (\\).
        /// </remarks>
        /// <example>
        /// digital_persona\klozin
        /// </example>
        [EnumMember]
        Sam = 3,

        /// <summary>
        /// Account name format used in Microsoft® Windows NT® 4.0.
        /// </summary>
        /// <example>"klozin"</example>
        [EnumMember]
        SamWithoutDomain = 4,

        /// <summary>
        /// Interface Identifier (IID) format.
        /// </summary>
        /// <remarks>
        /// GUID string that the IIDFromString function returns.
        /// </remarks>
        /// <example>{4fa050f0-f561-11cf-bdd9-00aa003a77b6}</example>
        [EnumMember]
        Iid = 5,

        /// <summary>
        /// UserName Principal Name format.
        /// </summary>
        /// <example>UserName@Example.Microsoft.com</example>
        [EnumMember]
        Upn = 6,

        /// <summary>
        /// Security Identifier (SID).
        /// </summary>
        /// <example>S-1-5-21-1004</example>
        [EnumMember]
        Sid = 7,

        /// <summary>
        /// Altus user name format.
        /// </summary>
        /// <example>user</example>
        /// <remarks>
        /// Altus user name format (user name associated with Altus identity database).
        /// </remarks>
        [EnumMember]
        Altus = 9
    }
}
