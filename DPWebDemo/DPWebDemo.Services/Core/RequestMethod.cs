using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo
{
    /// <summary>
    /// Http request method.
    /// </summary>
    internal enum RequestMethod
    {
        /// <summary>
        /// Get request. Get data by URL.
        /// </summary>
        Get,

        /// <summary>
        /// Post request. Get data using post parametrs.
        /// </summary>
        Post,

        /// <summary>
        /// Put request. Updaate data.
        /// </summary>
        Put,

        /// <summary>
        /// Delete request. Delete resource.
        /// </summary>
        Delete
    }
}
