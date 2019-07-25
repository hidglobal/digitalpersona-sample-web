using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services.Fingerprints
{
    /// <summary>
    /// Image compression enum.
    /// </summary>
    [DataContract]
    public enum FingerprintImageCompression
    {
        /// <summary>
        /// Data is not compressed
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// Jasper JPEG compression
        /// </summary>
        [EnumMember]
        JasperJpeg = 1,

        /// <summary>
        /// WSQ compression
        /// </summary>
        [EnumMember]
        Wsq = 2,

    }
}
