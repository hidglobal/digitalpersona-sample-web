using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services.Fingerprints
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public enum FingerprintImageType
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        BlackWhite = 1,

        [EnumMember]
        Grayscale = 2,

        [EnumMember]
        Color = 3,

    }
}
