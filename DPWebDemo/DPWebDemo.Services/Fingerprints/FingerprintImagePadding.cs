using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services.Fingerprints
{
    [DataContract]
    public enum FingerprintImagePadding
    {
        [EnumMember]
        NoPadding = 0,

        [EnumMember]
        LeftPadding = 1,

        [EnumMember]
        RightPadding = 2,

    }
}
