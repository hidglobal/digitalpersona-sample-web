using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Base calss for acces to Web services.
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        /// Server name or IP-address, that receive service requests.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Get full service address.
        /// </summary>
        /// <example>
        /// "https://example.com/DPWebAUTH/ServiceName.svc"
        /// </example>
        /// <returns>Full authentication service address string.</returns>
        protected abstract Uri GetRequestUri(string pathAppendix);

        protected ServiceBase()
        {
            ServerName = string.Empty;
        }

        /// <summary>
        /// Check service availability.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Ping()
        {
            try
            {
                var authAddress = GetRequestUri("/Ping");

                var request = HttpHelper.CreateHttpRequest(authAddress, RequestMethod.Get);

                await request.GetResponseData();

                return true;
            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }

        /// <summary>
        /// Get <see cref="ServiceException"/> from <see cref="WebException"/> and trace it.
        /// </summary>
        /// <returns>New corresponding <see cref="ServiceException"/>.</returns>
        protected static Exception GetServiceException(WebException webException)
        {
            if (webException == null)
                throw new ArgumentNullException("webException");

            var response = webException.Response;
            var webResponse = response as HttpWebResponse;
            if (webResponse != null)
                webResponse.TraceResponse();
            try
            {
                var error = response.GetResponseObject<ErrorData>();
               
                var standardException = Marshal.GetExceptionForHR(error.ErrorCode);
                if (standardException != null)
                    return standardException;

                return new ServiceException(error, webException);
            }
            catch (Exception)
            {
                Debug.WriteLine(webException.Message);
                return new ServiceException("Unknown exception", webException);
            }

        }
    }
}
