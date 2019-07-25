using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPWebDemo.Services.Biometric;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Information to Fingerprint authentication.
    /// </summary>
    public class FingerprintAuthenticationData
    {   
        /// <summary>
        /// Fingerprint <see cref="BiometricSample"/> that contains biometric information.
        /// </summary>
        public BiometricSample BiometricSample { get; private set; }
      
        /// <summary>
        /// Initialize new instance of <see cref="FingerprintAuthenticationData"/> with specific <see cref="BiometricSample"/> data.
        /// </summary>
        /// <param name="biometricSample">Biometric data.</param>
        public FingerprintAuthenticationData(BiometricSample biometricSample)
        {
            if (biometricSample == null)
                throw new ArgumentNullException("biometricSample");

            BiometricSample = biometricSample;
        }
    }
}
