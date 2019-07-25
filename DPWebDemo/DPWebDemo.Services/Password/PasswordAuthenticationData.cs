using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{

    /// <summary>
    /// Information to password authentication.
    /// </summary>
    public class PasswordAuthenticationData : DataObject
    {
        /// <summary>
        /// Password value.
        /// </summary>
        public string Password { get; private set; }
        
        /// <summary>
        /// Initialize new instance of <see cref="PasswordAuthenticationData"/> with specific password value.
        /// </summary>
        /// <param name="password">Password value.</param>
        public PasswordAuthenticationData(string password)
        {
            if (password == null) 
                throw new ArgumentNullException("password");

            Password = password;
        }
    }
}
