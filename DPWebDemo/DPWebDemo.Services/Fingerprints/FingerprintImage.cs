using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DPWebDemo.Services.Fingerprints
{
    [DataContract]
    public class FingerprintImage : DataObject
    {
        /// <summary>
        /// Version of Fingerprint Image format. Must be 1 in this version of document.
        /// </summary>
        [DataMember(Name = "Version")]
        public Byte Version { get; private set; }

        /// <summary>
        /// Header which specifies details of fingerprint imagine device.
        /// </summary>
        [DataMember(Name = "Header")]
        public FingerprintDataHeader Header { get; private set; }

        /// <summary>
        /// Format of fingerprint image. It details image size, image resolution, etc.
        /// </summary>
        [DataMember(Name = "Format")]
        public FingerprintImageFormat Format { get; private set; }

        /// <summary>
        /// Compression algorithm used to compressed fingerprint image. 
        /// </summary>
        [DataMember(Name = "Compression")]
        public FingerprintImageCompression Compression { get; private set; }

        /// <summary>
        /// Fingerprint image.
        /// </summary>
        [DataMember(Name = "Data")]
        [JsonConverter(typeof(Base64UrlConverter))]
        public byte[] Data { get; private set; }

        /// <summary>
        /// Inialize new onstance of <see cref="FingerprintImage"/> using specific data and compression.
        /// </summary>
        /// <param name="data">Image data.</param>
        /// <param name="compression">Image compression <see cref="FingerprintImageCompression"/>.</param>
        public FingerprintImage(byte[] data, FingerprintImageCompression compression)
        {
            if (data == null) 
                throw new ArgumentNullException("data");

            Data = data;
            Compression = compression;

            Header = new FingerprintDataHeader();
            Format = new FingerprintImageFormat();
            Version = 1;
        }
    }
}
