using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Information to PIN authentication.
    /// </summary>
    public class PinAuthenticationData : DataObject
    {
        /// <summary>
        /// Pin value.
        /// </summary>
        public string Pin { get;private set; }

        /// <summary>
        /// Initialize new instance of <see cref="PinAuthenticationData"/> with specific PIN value.
        /// </summary>
        /// <param name="pin">Pin value.</param>
        public PinAuthenticationData(string pin)
        {
            if (pin == null)
                throw new ArgumentNullException("pin");

            Pin = pin;
        }
    }
}
