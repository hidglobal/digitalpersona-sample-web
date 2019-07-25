using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services.Attributes
{
    /// <summary>
    /// Enumeration specifies type of attribute value(s).
    /// </summary>
    [DataContract]
    internal enum UserAttributeType
    {
        /// <summary>
        /// Attribute has Boolean value(s).
        /// </summary>
        [EnumMember]
        Boolean = 1,

        /// <summary>
        /// Attribute has Integer value(s). 
        /// </summary>
        [EnumMember]
        Integer = 2,

        /// <summary>
        /// Attribute has String value(s).
        /// </summary>
        [EnumMember]
        String = 3,

        /// <summary>
        /// Attribute has Blob (byte array) value(s)
        /// </summary>
        [EnumMember]
        Blob = 4,
    }
}
