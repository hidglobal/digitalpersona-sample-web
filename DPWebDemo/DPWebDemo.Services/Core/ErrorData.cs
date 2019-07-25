using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Error information provade by services.
    /// </summary>
    [DataContract]
    internal class ErrorData
    {
        /// <summary>
        /// Error code (HResult).
        /// </summary>
        [DataMember(Name="error_code")]
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Error description.
        /// </summary>
        [DataMember(Name="description")]
        public string Description { get; private set; }

         /// <summary>
        /// Private constructor to prevent create <see cref="ErrorData"/>.
        /// </summary>
        private ErrorData()
        {

        }
    }
}
