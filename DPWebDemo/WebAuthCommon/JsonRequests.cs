/*++
Copyright (c) 2014 Digital Persona, Inc. 

Module name: JsonRequests.cs     

--*/

using System;
using System.Runtime.Serialization;

namespace DPWebAUTHTest
{
    [DataContract]
    public class User
    {
        [DataMember]
        public String name { get; set; } // name of the user
        [DataMember]
        public UInt16 type { get; set; } // form (or type) of user name
    }
    [DataContract]
    public class Credential
    {
        [DataMember]
        public String id { get; set; } // unique id (Guid) of credential
        [DataMember]
        public String data { get; set; } // Base64 encoded credential data
    }
    [DataContract]
    class JsonAUTHRequest
    {
        [DataMember]
        public User user { get; set; } // user
        [DataMember]
        public Credential credential { get; set; } // cred
        public JsonAUTHRequest()
        {
            user = new User();
            credential = new Credential();
        }
        public JsonAUTHRequest(string name, UInt16 type, string id, string data)
        {
            user = new User();
            credential = new Credential();
            user.name = name;
            user.type = type;
            credential.id = id;
            credential.data = data;
        }
    }
    [DataContract]
    class JsonAUTHTicketRequest
    {
        [DataMember]
        public Ticket ticket { get; set; } // user
        [DataMember]
        public Credential credential { get; set; } // cred

        public JsonAUTHTicketRequest(String jwt, String id, String data)
        {
            ticket = new Ticket(jwt);
            credential = new Credential();
            credential.id = id;
            credential.data = data;
        }
    }
    [DataContract]
    class JsonIDENTRequest
    {
        [DataMember]
        public Credential credential { get; set; } // cred
        public JsonIDENTRequest()
        {
            credential = new Credential();
        }
        public JsonIDENTRequest(string id, string data)
        {
            credential = new Credential();
            credential.id = id;
            credential.data = data;
        }
    }
    [DataContract]
    class JsonSecretWriteRequest
    {
        [DataMember]
        public Ticket ticket { get; set; } // ticket
        [DataMember]
        public String secretName { get; set; } // secret name
        [DataMember]
        public String secretData{ get; set; } // secret data
    }
    [DataContract]
    class JsonSecretReadRequest
    {
        [DataMember]
        public Ticket ticket { get; set; } // ticket
        [DataMember]
        public String secretName { get; set; } // secret name
    }
    [DataContract]
    class JsonSecretReadResponse
    {
        [DataMember]
        public String ReadSecretResult { get; set; } // secret name
    }
    [DataContract]
    public enum LiveQuestionType
    {
        [EnumMember]
        REGULAR = 0,          // Regular Question
        [EnumMember]
        CUSTOM = 1,          // Custom Question
    }

    [DataContract]
    public class LiveQuestion
    {
        [DataMember]
        public Byte version { get; set; }    // version of Live Question class. Should be set to 1.
        [DataMember]
        public Byte number { get; set; }     // Question number
        [DataMember]
        public LiveQuestionType type { get; set; } // Question type
        [DataMember]
        public Byte lang_id { get; set; }        // Language ID should be used to display this question
        [DataMember]
        public Byte sublang_id { get; set; }     // Sublanguage ID should be used to display this question
        [DataMember]
        public UInt32 keyboard_layout { get; set; }  // Keyboard layout should be used to type answer
        [DataMember]
        public String text { get; set; }     // Custom question text. For custom questions only.
    }
    [DataContract]
    public class LiveAnswer
    {
        [DataMember]
        public Byte version { get; set; }    // version of Live Question class. Should be set to 1.
        [DataMember]
        public Byte number { get; set; }     // Question number
        [DataMember]
        public String text { get; set; }     // Question answer.
    }

////////////////////////////////////////////////////////////////////
// Fingerprints

    /* uImageType */
    [DataContract]
    public enum FpImageType
    {
        [EnumMember]
        UNKNOWN = 0,
        [EnumMember]
        BLACK_WHITE = 1,
        [EnumMember]
        GRAY_SCALE = 2,
        [EnumMember]
        COLOR = 3,
    }

    /* uPadding */
    [DataContract]
    public enum FpImagePadding
    {
        [EnumMember]
        NO_PADDING = 0,
        [EnumMember]
        LEFT_PADDING = 1,
        [EnumMember]
        RIGHT_PADDING = 2,
    }

    /* uPolarity */
    [DataContract]
    public enum FpImagePolarity
    {
        [EnumMember]
        UNKNOWN_POLARITY = 0,
        [EnumMember]
        NEGATIVE_POLARITY = 1,
        [EnumMember]
        POSITIVE_POLARITY = 2,
    }

    /* uRGBcolorRepresentation */
    [DataContract]
    public enum FpImageColorRepresantation
    {
        [EnumMember]
        NO_COLOR_REPRESENTATION = 0,
        [EnumMember]
        PLANAR_COLOR_REPRESENTATION = 1,
        [EnumMember]
        INTERLEAVED_COLOR_REPRESENTATION = 2,
    }

    [DataContract]
    public class FpDataHeader
    {
        [DataMember]
        public Byte uDataType { get; set; }  /* must be 1 which represents 2D image	*/
        [DataMember]
        public UInt64 DeviceId { get; set; } /* reserved for future use		        */
        /* FD_DEVICE_TYPE provides the following information:								*/
        /* 0000        -  05BA        - 0001         - 01              - 04					*/
        /* reserved       vendor Id   - product Id   - major revision  - minor revision		*/
        [DataMember]
        public UInt64 DeviceType { get; set; }
        /* image acquisition or device testing may take some time. Progress indicator		*/
        /* gives a feed back on the progress of the transaction.								*/
        [DataMember]
        public Byte iDataAcquisitionProgress { get; set; } 	/* should be 100					*/
    }

    [DataContract]
    public class FpImageFormat
    {
        [DataMember]
        public Byte uDataType { get; set; }               /* is 1 which represents 2D image	  		  */
        [DataMember]
        public FpImageType uImageType { get; set; }       /* B&W, gray or color images                */
        [DataMember]
        public Int32 iWidth { get; set; }                 /* image width [pixel]                       */
        [DataMember]
        public Int32 iHeight { get; set; }                /* image height [pixel]                      */
        [DataMember]
        public Int32 iXdpi { get; set; }                  /* X resolution [DPI]                        */
        [DataMember]
        public Int32 iYdpi { get; set; }                  /* Y resolution [DPI]                        */
        [DataMember]
        public UInt32 uBPP { get; set; }                  /* number of bits per pixel                  */
        [DataMember]
        public FpImagePadding uPadding { get; set; }      /* right or left padding                     */
        [DataMember]
        public UInt32 uSignificantBpp { get; set; }       /* number of significant bits per pixel      */
        [DataMember]
        public FpImagePolarity uPolarity { get; set; }    /* positive=black print on white background  */
        [DataMember]
        public FpImageColorRepresantation uRGBcolorRepresentation { get; set; }
        [DataMember]
        public UInt32 uPlanes { get; set; }                 /* color planes.                             */
    }

    [DataContract]
    public enum FpImageCompression
    {
        [EnumMember]
        NONE = 0,                     // Data is not compressed
        [EnumMember]
        JASPER_JPEG = 1,              // Jasper JPEG compression
    }

    [DataContract]
    public class FpImage
    {
        public FpImage()
        {
            Header = new FpDataHeader();
            Format = new FpImageFormat();
        }
        [DataMember]
        public Byte Version { get; set; }
        [DataMember]
        public FpDataHeader Header { get; set; }
        [DataMember]
        public FpImageFormat Format { get; set; }
        [DataMember]
        public FpImageCompression Compression { get; set; }
        [DataMember]
        public String Data { get; set; }
    }
//---------------------------
    [DataContract]
    public class FpFeatureSetFormat
    {
        public FpFeatureSetFormat()
        {
            FormatOwner = 51; // DigitalPersona engine
            FormatID = 0;
        }
        [DataMember]
        public int FormatOwner { get; set; }
        [DataMember]
        public int FormatID { get; set; }
    }
    [DataContract]
    public class FpFeatureSetHeader
    {
        public FpFeatureSetHeader()
        {
            Factor = 8; // Fingerprint
            Format = new FpFeatureSetFormat();
            Type = 2; 		// by default, Feature Set
            Purpose = 0; 	// Any purpose
            Quality = -1;
            Encryption = 0;	// Unencrypted 
        }
        [DataMember]
        public int Factor { get; set; }
        [DataMember]
        public FpFeatureSetFormat Format { get; set; }
        [DataMember]
        public int Type { get; set; }
        [DataMember]
        public int Purpose { get; set; }
        [DataMember]
        public int Quality { get; set; }
        [DataMember]
        public int Encryption { get; set; }
    }

    [DataContract]
    public class FpFeatureSet
    {
        public FpFeatureSet()
        {
            Version = 1;
            Header = new FpFeatureSetHeader();
        }
        [DataMember]
        public Byte Version { get; set; }
        [DataMember]
        public FpFeatureSetHeader Header { get; set; }
        [DataMember]
        public String Data { get; set; }
    }
}
