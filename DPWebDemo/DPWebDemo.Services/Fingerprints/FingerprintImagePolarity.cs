using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services.Fingerprints
{
    [DataContract]
    public enum FingerprintImagePolarity
    {
        [EnumMember]
        UnknownPolarity = 0,

        [EnumMember]
        NegativePolarity = 1,

        [EnumMember]
        PositivePolarity = 2,

    }
}
