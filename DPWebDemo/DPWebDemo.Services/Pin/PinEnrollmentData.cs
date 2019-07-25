using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Information to PIN enrollment.
    /// </summary>
    public class PinEnrollmentData : DataObject
    {
        /// <summary>
        /// PIN string.
        /// </summary>
        public string Pin { get; private set; }

        /// <summary>
        /// Initialize new instance of <see cref="PinEnrollmentData"/> with specific PIN value.
        /// </summary>
        /// <param name="pin">Pin value.</param>
        public PinEnrollmentData(string pin)
        {
            if (pin == null) 
                throw new ArgumentNullException("pin");

            Pin = pin;
        }
    }
}
