using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Http body parameter.
    /// </summary>
    internal class BodyParameter
    {
        /// <summary>
        /// Parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameter value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Initialize new instance of <see cref="BodyParameter"/>.
        /// </summary>
        public BodyParameter()
        {
        }

        /// <summary>
        /// Initialize new instance of <see cref="BodyParameter"/> with specific name and value.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        public BodyParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
