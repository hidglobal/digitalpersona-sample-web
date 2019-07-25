using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Information to OTP enrollment.
    /// </summary>
    /// <remarks>
    /// If OTP code cannot be verified, enrollment operation will fail with the following error:
    /// “The operation being requested was not performed because the user has not been authenticated.”.
    ///  NOTE: verification can fail only for two reasons:
    ///  1) user mistyped OTP code or 
    ///  2) clocks on phone and Altus Server are not synchronized.
    /// </remarks>
    [DataContract]
    public class OtpEnrollmentData : DataObject
    {
        /// <summary>
        /// OTP verification code. 
        /// </summary>
        [DataMember(Name = "otp")]
        public string OneTimePassword { get; set; }
        /// <summary>
        /// TOTP key.
        /// </summary>
        [DataMember(Name = "key")]
        [JsonConverter(typeof(Base64UrlConverter))]
        public byte[] Key { get; private set; }

        /// <summary>
        /// Initialize new instance of <see cref="OtpEnrollmentData"/> with specific One-Time Password and TOTP key value.
        /// </summary>
        /// <param name="otp">OTP verification code.</param>
        /// <param name="key">TOTP key.</param>
        public OtpEnrollmentData(string otp, byte[] key)
        {
            if (otp == null)
                throw new ArgumentNullException("otp");
            
            if (key == null) 
                throw new ArgumentNullException("key");


            OneTimePassword = otp;
            Key = key;
        }
    }
}
