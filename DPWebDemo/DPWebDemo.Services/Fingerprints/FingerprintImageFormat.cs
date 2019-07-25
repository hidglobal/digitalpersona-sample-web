using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services.Fingerprints
{
    /// <summary>
    /// Image format.
    /// </summary>
    [DataContract]
    public class FingerprintImageFormat : DataObject
    {
        /// <summary>
        /// is 1 which represents 2D image
        /// </summary>
        [DataMember(Name = "uDataType")]
        public FingerprintDataType DataType { get; set; }

        /// <summary>
        /// B&W, gray or color images
        /// </summary>
        [DataMember(Name = "uImageType")]
        public FingerprintImageType ImageType { get; set; }

        /// <summary>
        /// image width [pixel].
        /// </summary>
        [DataMember(Name = "iWidth")]
        public int Width { get; set; }

        /// <summary>
        /// image height [pixel]
        /// </summary>
        [DataMember(Name = "iHeight")]
        public int Height { get; set; }

        /// <summary>
        /// X resolution [DPI]
        /// </summary>
        [DataMember(Name = "iXdpi")]
        public int DpiX { get; set; }

        /// <summary>
        /// Y resolution [DPI]
        /// </summary>
        [DataMember(Name = "iYdpi")]
        public int DpiY { get; set; }

        /// <summary>
        /// Number of bits per pixel 
        /// </summary>
        [DataMember(Name = "uBPP")]
        public int Bpp { get; set; }

        /// <summary>
        /// right or left padding
        /// </summary>
        [DataMember(Name = "uPadding")]
        public FingerprintImagePadding Padding { get; set; }

        /// <summary>
        /// Number of significant bits per pixel
        /// </summary>
        [DataMember(Name = "uSignificantBpp")]
        public uint SignificantBpp { get; set; }

        /// <summary>
        /// positive=black print on white background
        /// </summary>
        [DataMember(Name = "uPolarity")]
        public FingerprintImagePolarity Polarity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "uRGBcolorRepresentation")]
        public FingerprintImageColorRepresentation ColorRepresentation { get; set; }

        /// <summary>
        /// Color planes.
        /// </summary>
        [DataMember(Name = "uPlanes")]
        public uint Planes { get; set; }

        /// <summary>
        /// Initialize new instance of <see cref="FingerprintImageFormat"/>.
        /// </summary>
        public FingerprintImageFormat()
        {
            DataType = FingerprintDataType.TwoDimensions;
            ColorRepresentation = FingerprintImageColorRepresentation.NoColorRepresentation;
        }
    }
}
