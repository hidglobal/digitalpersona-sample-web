using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DPWebDemo.Services.Attributes
{
    /// <summary>
    /// User atribute representation. 
    /// Available values types: <see cref="T:byte[]"/> , <see cref="string"/>, <see cref="int"/>, <see cref="bool"/>, <see cref="IEnumerable{byte[]}"/>, 
    /// <see cref="IEnumerable{string}"/>, <see cref="IEnumerable{int}"/>, <see cref="IEnumerable{bool}"/>.
    /// </summary>
    /// <remarks>
    /// This class contains some public (biographic) information about user, like user surname, date of birth, e-mail address, etc.
    /// </remarks>
    [DataContract]
    internal class UserAttribute
    {
        private static readonly Dictionary<UserAttributeType, Type> attributeTypes = new Dictionary<UserAttributeType, Type>
        {
            { UserAttributeType.Blob, typeof(byte[]) },
            { UserAttributeType.Boolean, typeof(bool) },
            { UserAttributeType.String, typeof(string) },
            { UserAttributeType.Integer, typeof(int) }
        };

        /// <summary>
        /// Type property name in JSON.
        /// </summary>
        internal const string TypeDataMemberString = "type";

        /// <summary>
        /// Values property name in JSON.
        /// </summary>
        internal const string ValuesDataMemberString = "values";

        /// <summary>
        /// Type of Attribute value(s).
        /// </summary>
        [DataMember(Name = TypeDataMemberString)]
        public UserAttributeType Type { get; private set; }

        /// <summary>
        /// Values of attribute. We assume all attributes are multivalued because singlevalued attributes are subsystem of multivalued attributes
        /// </summary>
        [DataMember(Name = ValuesDataMemberString)]
        public virtual IEnumerable<object> Values { get; protected set; }

        /// <summary>
        /// Attribute is multi value.
        /// </summary>
        [IgnoreDataMember]
        public bool IsMultiValueAttribute
        {
            get { return Values.Count() > 1; }
        }

        /// <summary>
        /// Create new emty instance of <see cref="UserAttribute"/>.
        /// </summary>
        private UserAttribute()
        {

        }

        /// <summary>
        /// Create new instance of <see cref="UserAttribute"/> using specific value.
        /// </summary>
        /// <param name="value">Attribute value. 
        /// Available values types: <see cref="T:byte[]"/> , <see cref="string"/>, <see cref="int"/>, <see cref="bool"/>, <see cref="IEnumerable{byte[]}"/>, 
        /// <see cref="IEnumerable{string}"/>, <see cref="IEnumerable{int}"/>, <see cref="IEnumerable{bool}"/>.
        /// </param>
        private UserAttribute(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value is bool || value is IEnumerable<bool>)
                Type = UserAttributeType.Boolean;
            else if (value is int || value is IEnumerable<int>)
                Type = UserAttributeType.Integer;
            else if (value is string || value is IEnumerable<string>)
                Type = UserAttributeType.String;
            else
                throw new ArgumentException("Type must be string, int, bool, IEnumerable<string>, IEnumerable<int>, IEnumerable<bool> instead " + value.GetType());

            if (value is IEnumerable && !(value is string))
                Values = (IEnumerable<object>)value;
            else
                Values = new[] { value };
        }

        /// <summary>
        /// Get see <see cref="UserAttributeType"/> corresponding specific <see cref="Type"/>.
        /// </summary>
        /// <param name="type">Type to convert.</param>
        /// <returns><see cref="UserAttributeType"/> corresponding specific <see cref="Type"/>.</returns>
        public static UserAttributeType GetUserAttributeType(Type type)
        {
            if (!attributeTypes.ContainsValue(type))
                throw new ArgumentException("Type must be byte[], string, int, bool instead " + type);

            return attributeTypes.Single(p => p.Value == type).Key;
        }

        /// <summary>
        /// Get see <see cref="Type"/> corresponding specific <see cref="UserAttributeType"/>.
        /// </summary>
        /// <param name="type"><see cref="UserAttributeType"/>  to convert.</param>
        /// <returns><see cref="Type"/> corresponding specific type</returns>
        public static Type GetTypeByUserAttribute(UserAttributeType type)
        {
            return attributeTypes[type];
        }

        /// <summary>
        /// Parse json to <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Available values types: <see cref="T:byte[]"/> , <see cref="string"/>, <see cref="int"/>, <see cref="bool"/>.</typeparam>
        /// <param name="json">JSON to parse.</param>
        /// <returns>Corresponding <see cref="UserAttribute"/>.</returns>
        public static IEnumerable<T> ParseValues<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return Enumerable.Empty<T>();

            var userAttribute = JObject.Parse(json);

            var userAttributeType = userAttribute[TypeDataMemberString].ToObject<UserAttributeType>();

            if (userAttributeType != GetUserAttributeType(typeof(T)))
                throw new ArgumentException("T must be " + GetTypeByUserAttribute(userAttributeType));

            var values = userAttribute[ValuesDataMemberString];

            if (userAttributeType != UserAttributeType.Blob)
                return values.ToObject<IEnumerable<T>>();

            return (IEnumerable<T>)userAttribute.ToObject<BlobUserAttribute>().Values;
        }

        /// <summary>
        /// Create single <see cref="UserAttribute"/> using specific value.
        /// </summary>
        /// <param name="value">Attribute value. 
        /// Available values types: <see cref="T:byte[]"/> , <see cref="string"/>, <see cref="int"/>, <see cref="bool"/>, <see cref="IEnumerable{byte[]}"/>, 
        /// <see cref="IEnumerable{string}"/>, <see cref="IEnumerable{int}"/>, <see cref="IEnumerable{bool}"/>.
        /// </param>
        public static UserAttribute Create(object value)
        {
            if (value == null)
                return new UserAttribute();

            var ua = new UserAttribute();
            if (value is byte[] || value is IEnumerable<byte[]>)
            {
                ua.Type = UserAttributeType.Blob;
                return new BlobUserAttribute(value);
            }
            return new UserAttribute(value);
        }

        /// <summary>
        /// Byte[] attribute.
        /// </summary>
        internal class BlobUserAttribute : UserAttribute
        {
            /// <summary>
            /// Values of attribute. We assume all attributes are multivalued because singlevalued attributes are subsystem of multivalued attributes
            /// </summary>
            [DataMember(Name = ValuesDataMemberString)]
            [JsonProperty(ItemConverterType = typeof(Base64UrlConverter))]
            public override IEnumerable<object> Values { get; protected set; }

            public BlobUserAttribute(object value)
            {
                Type = UserAttributeType.Blob;
                if (value is IEnumerable<byte[]>)
                    Values = (IEnumerable<object>)value;
                else
                    Values = new[] { value };
            }

        }
    }
}
