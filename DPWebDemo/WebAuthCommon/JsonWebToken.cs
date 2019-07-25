/*++
Copyright (c) 2014 Digital Persona, Inc. 

Module name: IDForm.cs
        
DP's sample application for WEB based authentication.     

JsonWebToken.
--*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace DPWebAUTHTest
{
    public class JsonWebToken
    {
        public String JwtHeader;
        public String JwtClaims;
        public String JwtSignature;

        public JsonWebToken(String jwt)
        {
            string[] parts = jwt.Split(new Char[] { '.' });
            if (parts.Length != 3)
            {
                throw new Exception("Invalid token format. Expected Envelope.Claims.Signature");
            }

            if (string.IsNullOrEmpty(parts[0]))
            {
                throw new Exception("Invalid token format. Envelope must not be empty");
            }

            if (string.IsNullOrEmpty(parts[1]))
            {
                throw new Exception("Invalid token format. Claims must not be empty");
            }

            if (string.IsNullOrEmpty(parts[2]))
            {
                throw new Exception("Invalid token format. Signature must not be empty");
            }
            JwtHeader = parts[0];
            JwtClaims = parts[1];
            JwtSignature = parts[2];
        }

        public JsonClaims Claims()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JsonClaims));
            using (MemoryStream stream = new MemoryStream(Base64UrlDecode(JwtClaims)))
            {
                return (JsonClaims)ser.ReadObject(stream);
            }
        }
        // from JWT spec
        public static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
        // from JWT spec
        public static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromSecondsSinceUnixEpoch(long seconds)
        {
            return UnixEpoch + TimeSpan.FromSeconds(seconds);
        }
    }
    [DataContract]
    public class JwtCredential
    {
        [DataMember]
        public String id;
        [DataMember]
        public UInt32 time;
    }
    [DataContract]
    public class JsonClaims
    {
        [DataMember]
        public String jti;  // Unique ID
        [DataMember]
        public String iss;  // 
        [DataMember]
        public String dom;  // 
        [DataMember]
        public UInt32 iat;
        [DataMember]
        public String sub;
        [DataMember]
        public String uid;
        [DataMember]
        public List<JwtCredential> crd;
    }
    [DataContract]
    public class Ticket
    {
        public Ticket(String strJWT)
        {
            jwt = strJWT;
        }
        [DataMember]
        public String jwt { get; set; } // JSON Web Token
    }
    [DataContract]
    public class AuthenticateUserResponse
    {
        [DataMember]
        public Ticket AuthenticateUserResult{ get; set; } // JSON Web Token
    }
    [DataContract]
    public class IdentifyUserResponse
    {
        [DataMember]
        public Ticket IdentifyUserResult { get; set; } // JSON Web Token
    }
    [DataContract]
    public class AuthenticateTicketResponse
    {
        [DataMember]
        public Ticket AuthenticateUserTicketResult { get; set; } // JSON Web Token
    }
    [DataContract]
    public class LiveQuestionResponse
    {
        [DataMember]
        public String GetEnrollmentDataResult { get; set; } // JSON Web Token
    }
    [DataContract]
    public class Test
    {
        [DataMember]
        public DateTime time { get; set; } // JSON Web Token
    }
}
