using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DPWebDemo.Services.Biometric;
using Newtonsoft.Json;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Information to Fingerprint enrollment.
    /// </summary>
    public class FingerprintEnrollmentData : DataObject
    {
        /// <summary>
        /// Fingerprint position.
        /// </summary>
        [DataMember(Name = "position")]
        public FingerPosition Position { get; private set; }

        /// <summary>
        /// Bio samples to enroll.
        /// Fingerprint data to enroll for such position. List could include from one <see cref="BiometricSample"/> to any <see cref="BiometricSample"/>.
        /// </summary>
        [DataMember(Name = "samples")]
        public IEnumerable<BiometricSample> Samples { get; private set; }

        /// <summary>
        /// Initialize new instance of <see cref="FingerprintEnrollmentData"/> with pecific finger index <see cref="FingerPosition"/>.
        /// </summary>
        /// <param name="position">Finger index.</param>
        [JsonConstructor]
        internal FingerprintEnrollmentData(FingerPosition position)
        {
            Position = position;
        }

        /// <summary>
        /// Initialize new instance of <see cref="FingerprintEnrollmentData"/> with pecific finger index <see cref="FingerPosition"/> and <see cref="BiometricSample"/>.
        /// </summary>
        /// <param name="position">Finger index.</param>
        /// <param name="samples">Collection of <see cref="BiometricSample"/> for specific finger.</param>
        public FingerprintEnrollmentData(FingerPosition position, IEnumerable<BiometricSample> samples)
            : this(position)
        {
            if (samples == null)
                throw new ArgumentNullException("samples");

            Samples = samples.ToArray();
        }
    }
}
