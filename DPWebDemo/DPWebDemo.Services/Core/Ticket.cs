using System.Runtime.Serialization;

namespace DPWebDemo.Services
{
    /// <summary>
    /// JSON Web Token. Provide this ticket to all secure operations for identify user or officer.
    /// </summary>
    /// <remarks>
    /// The format of returned Ticket upon successful authentication would JSON Web Token.
    /// Specification of JWT is detailed here https://tools.ietf.org/html/draft-jones-json-web-token-10.
    /// specification of JWT will be returned by DigitalPersona Web Authentication Service version 1.0.
    /// Standard JWT has three parts: 1) Header; 2) Claims and 3) Signature.
    /// All three parts created separately as UTF-8 strings, then Base64url encoded and finally concatenated in order above with period characters between the parts yields this complete JWT.
    ///Base64url Encoding: for the purposes of this specification, this term always refers to the URL- and filename-safe Base64 encoding described in RFC 4648 [RFC4648], Section 5, with the (non URL-safe) '=' padding characters omitted, as permitted by Section 3.2.
    /// </remarks>
    [DataContract]
    public class Ticket : DataObject
    {
        /// <summary>
        /// JSON Web Token data.
        /// </summary>
        [DataMember(Name = "jwt")]
        public string Data { get; set; }

        /// <summary>
        /// Internal constructor to prevent creating <see cref="Ticket"/> instance.
        /// </summary>
        internal Ticket()
        {
            
        }
    }
}
