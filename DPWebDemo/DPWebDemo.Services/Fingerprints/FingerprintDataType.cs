using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services.Fingerprints
{
    [DataContract]
    public enum FingerprintDataType : byte
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0,

        /// <summary>
        /// 2D Image.
        /// </summary>
        TwoDimensions = 1
    }
}
