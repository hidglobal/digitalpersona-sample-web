using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Information to OTP authentication.
    /// </summary>
    public class OtpAuthenticationData : DataObject
    {
        /// <summary>
        /// One time password value. (6 digits).
        /// </summary>
        public string OneTimePassword { get; private set; }
       
        /// <summary>
        /// Initialize new instance of <see cref="OtpAuthenticationData"/> with specific OTP value.
        /// </summary>
        /// <param name="oneTimePassword">One Time Password value.</param>
        public OtpAuthenticationData(string oneTimePassword)
        {
            if (oneTimePassword == null)
                throw new ArgumentNullException("oneTimePassword");

            OneTimePassword = oneTimePassword;
        }
    }
}
