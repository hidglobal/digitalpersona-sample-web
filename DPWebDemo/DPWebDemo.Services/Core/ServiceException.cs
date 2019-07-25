using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Represents errors that occur during service operation execution.
    /// </summary>
    public class ServiceException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        internal ServiceException()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified <see cref="ErrorData"/>.
        /// </summary>
        /// <param name="message">The <see cref="ErrorData"/> that describes the error. </param>
        internal ServiceException(ErrorData message)
            : base(message.Description)
        {
            HResult = message.ErrorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified <see cref="ErrorData"/> and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The <see cref="ErrorData"/> that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        internal ServiceException(ErrorData message, Exception innerException)
            : base(message.Description, innerException)
        {
            HResult = message.ErrorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified message.
        /// </summary>
        /// <param name="message">Message that explains the reason for the exception. </param>
        internal ServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">Message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        internal ServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
