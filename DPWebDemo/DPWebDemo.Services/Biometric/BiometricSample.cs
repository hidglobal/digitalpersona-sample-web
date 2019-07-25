using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DPWebDemo.Services.Biometric
{
    /// <summary>
    /// BioSample class represent general biometric sample.
    /// </summary>
    [DataContract]
    public class BiometricSample : DataObject
    {
        /// <summary>
        /// Version of Biometric Sample format. 
        /// Must be set to 1 in this version.
        /// </summary>
        [DataMember(Name = "Version")]
        public byte Version { get; private set; }

        /// <summary>
        /// Header which specifies details of format of Biometric data .
        /// </summary>
        [DataMember(Name = "Header")]
        public BiometricSampleHeader Header { get; private set; }

        /// <summary>
        /// Base64url encoded biometric sample data.
        /// </summary>
        [DataMember(Name = "Data")]
        [JsonConverter(typeof(Base64UrlConverter))]
        public object Data { get; private set; }

        /// <summary>
        /// Initialize object by default values.
        /// </summary>
        private BiometricSample()
        {
            Version = 1;
        }

        /// <summary>
        /// Initialize new instance of <see cref="BiometricSample"/>, using specific data.
        /// </summary>
        /// <param name="data">Biometric data.</param>
        public BiometricSample(object data)
            : this()
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;
        }

        /// <summary>
        /// Initialize new instance of <see cref="BiometricSample"/>, using specific data and header.
        /// </summary>
        /// <param name="data">Biometric data.</param>
        /// <param name="headerType">Header type.</param>
        public BiometricSample(object data, BiometricSampleHeaderType headerType)
            : this(data)
        {
            Header = BiometricSampleHeader.GetHeader(headerType);
        }
    }
}
