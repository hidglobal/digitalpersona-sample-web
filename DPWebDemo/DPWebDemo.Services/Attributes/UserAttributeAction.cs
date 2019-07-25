using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services.Attributes
{
    /// <summary>
    /// Action which could be taken with attribute in PutUserAttribute service method call.
    /// </summary>
    [DataContract]
    internal enum UserAttributeAction
    {
        /// <summary>
        /// Attribute must be cleared. In this case attributeData argument of PutUserAttribute method will be ignored and can be "null". 
        /// </summary>
        [EnumMember]
        Clear = 1,

        /// <summary>
        /// Attribute will be updated. All previous data in attribute will be cleared. 
        /// </summary>
        [EnumMember]
        Update = 2,

        /// <summary>
        /// The data will be appended to data which already exists in attribute. Makes sense only for multivalued attributes.
        /// </summary>
        [EnumMember]
        Append = 3,

        /// <summary>
        /// The data provided in attributeData argument of PutUserAttribute method will be deleted from attribute. Makes sense only for multivalued attributes.
        /// </summary>
        [EnumMember]
        Delete = 4
    }
}
