using System;
using System.Runtime.Serialization;

namespace DPWebDemo.Services
{
    /// <summary>
    /// User name, that identify user.
    /// </summary>
    [DataContract]
    public class UserName : DataObject
    {
        /// <summary>
        /// User Name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; private set; }

        /// <summary>
        /// UserName name format.
        /// </summary>
        [DataMember(Name = "type")]
        public UserNameFormat Format { get; private set; }

        /// <summary>
        /// Constructor. Initilialize new instance of <see ref="UserName"/>.
        /// </summary>
        /// <param name="name">UserName name in specific format.</param>
        /// <param name="userNameFormat">Format of user name.</param>
        public UserName(string name, UserNameFormat userNameFormat)
        {
            Name = name;
            Format = userNameFormat;
        }

        /// <summary>
        /// Parse string to user. Auto detect user name format.
        /// </summary>
        /// <param name="userName">UserName name to parse.</param>
        /// <returns>UserName.</returns>
        public static UserName Parse(string userName)
        {
            if (userName == null)
                throw new ArgumentNullException("userName");
            
            UserNameFormat userNameformat;

            if (userName.Contains("@"))
                userNameformat = UserNameFormat.Upn;
            else if (userName.Contains("\\"))
                userNameformat = UserNameFormat.Sam;
            else
                userNameformat = UserNameFormat.Altus;

            return new UserName(userName, userNameformat);
        }

    }
}
