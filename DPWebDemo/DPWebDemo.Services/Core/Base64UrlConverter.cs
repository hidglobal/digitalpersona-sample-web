using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Convert any object to URL Base64 string using RFC 4648 Chapter 5.
    /// </summary>
    internal class Base64UrlConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// 
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param><param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var decodedData = (string)reader.Value;

            if (decodedData == null)
                return null;

            decodedData = decodedData.Replace('-', '+');
            decodedData = decodedData.Replace('_', '/');

            switch (decodedData.Length % 4)
            {
                case 0: break;
                case 2: decodedData += "=="; break;
                case 3: decodedData += "="; break;
                default: throw new InvalidOperationException("Illegal base64url string!");
            }

            var array = Convert.FromBase64String(decodedData);

            if (objectType == typeof(byte[]))
                return array;
            if (objectType == typeof(string))
                return Encoding.UTF8.GetString(array, 0, array.Length);

            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(array, 0, array.Length), objectType);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var array = value as byte[];

            if (array == null)
            {
                var strValue = value as string;
                if (strValue == null)
                    strValue = JsonConvert.SerializeObject(value);
                array = Encoding.UTF8.GetBytes(strValue);
            }


            var encodedData = Convert.ToBase64String(array);
            encodedData = encodedData.TrimEnd('=');
            encodedData = encodedData.Replace('+', '-');
            encodedData = encodedData.Replace('/', '_');
            writer.WriteValue(encodedData);
        }
    }
}
