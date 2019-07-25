using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services.Fingerprints
{
    [DataContract]
    public class FingerprintDataHeader : DataObject
    {
        /// <summary>
        ///  Must be 1 which represents 2D image
        /// </summary>
        [DataMember(Name = "uDataType")]
        public FingerprintDataType DataType { get; private set; }

        /// <summary>
        /// Reserved for future use FD_DEVICE_TYPE
        /// </summary>
        [DataMember(Name = "DeviceId")]
        public UInt64 DeviceId { get; set; }

        /// <summary>
        /// Provides the following information:
        ///  0000        -  05BA        - 0001         - 01              - 04
        /// reserved       vendor Id   - product Id   - major revision  - minor revision
        /// </summary>
        [DataMember(Name = "DeviceType")]
        public UInt64 DeviceType { get; set; }

        /// <summary>
        /// Image acquisition or device testing may take some time. Progress indicator gives a feed back on the progress of the transaction.	
        /// should be 100	
        /// </summary>
        [DataMember(Name = "iDataAcquisitionProgress")]
        public byte DataAcquisitionProgress { get; private set; }

        /// <summary>
        /// Initialize new onstance of <see cref="FingerprintDataHeader"/>.
        /// </summary>
        public FingerprintDataHeader()
        {
            DataType = FingerprintDataType.TwoDimensions;
            DataAcquisitionProgress = 100;
        }
    }
}
