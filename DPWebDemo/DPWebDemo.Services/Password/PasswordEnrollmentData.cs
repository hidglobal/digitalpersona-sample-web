using System;
using System.Runtime.Serialization;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Information to password enrollment.
    /// </summary>
    [DataContract]
    public class PasswordEnrollmentData : DataObject
    {
        /// <summary>
        /// UserName’s old password. This parameter is optional and can be null, in this case Password Reset operation would be used. 
        /// If old password is presented, Password Change operation will be used and owner argument of EnrollUserCredentials could be null.
        /// </summary>
        [DataMember(Name = "oldPassword")]
        public string OldPassword { get; private set; } // Old password 

        /// <summary>
        /// Users new password. Password must satisfy password policy otherwise this call will fail.
        /// </summary>
        [DataMember(Name = "newPassword")]
        public string NewPassword { get; private set; } // New password 

        /// <summary>
        /// Initialize new instance of <see cref="PasswordEnrollmentData"/> with specific old and new password values.
        /// </summary>
        /// <param name="oldPassword">Current password value.</param>
        /// <param name="newPassword">New password value.</param>
        public PasswordEnrollmentData(string oldPassword, string newPassword)
        {
            if (oldPassword == null) 
                throw new ArgumentNullException("oldPassword");
           
            if (newPassword == null)
                throw new ArgumentNullException("newPassword");

            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
}
