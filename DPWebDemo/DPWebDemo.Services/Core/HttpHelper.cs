using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Helper class for web request/response.
    /// </summary>
    internal static class HttpHelper
    {
        public static HttpWebRequest CreateHttpRequest(Uri url, RequestMethod requestMethod)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = requestMethod.ToString().ToUpperInvariant();
            request.ContentType = "application/json";
            request.Accept = "application/json; charset=utf-8";

            TraceRequest(request);

            return request;
        }

        #region RequestParameters

        public static async Task WritePostParameters(this HttpWebRequest request, params BodyParameter[] paramerters)
        {
            var jObject = new JObject();

            foreach (var parameter in paramerters)
            {
                if (parameter.Value == null)
                    jObject[parameter.Name] = null;
                else
                    jObject[parameter.Name] = JToken.FromObject(parameter.Value);
                var cred = parameter.Value as ICredential;

                if (cred != null)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("Decoded credential data:");
                    Debug.WriteLine(JsonConvert.SerializeObject(cred.Data, Formatting.Indented));
                    Debug.WriteLine("");
                }
            }

            var json = jObject.ToString();

            Debug.WriteLine(jObject.ToString());

            var requestData = Encoding.UTF8.GetBytes(json);
            using (var stream = await request.GetRequestStreamAsync())
                stream.Write(requestData, 0, requestData.Length);
        }

        public static async Task WritePostParameters(this HttpWebRequest request, string name, object value)
        {
            await WritePostParameters(request, new BodyParameter(name, value));
        }

        public static async Task WritePostParameters(this HttpWebRequest request, string name1, object value1, string name2, object value2)
        {
            await WritePostParameters(request, new BodyParameter(name1, value1), new BodyParameter(name2, value2));
        }

        #endregion

        #region Get response data

        public static async Task<string> GetResponseData(this HttpWebRequest request)
        {
            var response = (HttpWebResponse)await request.GetResponseAsync();

            response.TraceResponse();

            var responseData = response.GetResponseData();

            return responseData;
        }

        public static async Task<T> GetResponseData<T>(this HttpWebRequest request, string parameterName, bool useBase64UrlDecoding = false)
        {
            var responseData = await request.GetResponseData();

            var jObject = JObject.Parse(responseData);

            if (useBase64UrlDecoding)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new Base64UrlConverter());

                var decodedObject = jObject[parameterName].ToObject<T>(JsonSerializer.Create(settings));

                Debug.WriteLine(string.Empty);

                Debug.WriteLine("Decoded data:");

                Debug.WriteLine(JsonConvert.SerializeObject(decodedObject, Formatting.Indented));

                return decodedObject;
            }
            return jObject[parameterName].ToObject<T>();
        }

        public static string GetResponseData(this WebResponse response)
        {
            if (response.ContentLength == 0)
                return null;

            using (var stream = response.GetResponseStream())
            {
                var buffer = new byte[response.ContentLength];
                var count = stream.Read(buffer, 0, (int)response.ContentLength);

                var str = Encoding.UTF8.GetString(buffer, 0, count);

                Debug.WriteLine(str);

                return str;
            }
        }

        public static T GetResponseObject<T>(this WebResponse response)
        {
            var responseData = response.GetResponseData();

            return JsonConvert.DeserializeObject<T>(responseData);
        }

        #endregion

        #region Tracing

        public static void TraceRequest(this HttpWebRequest request)
        {
            var result = new List<string>();

            result.Add(request.Method + " " + request.RequestUri.PathAndQuery);
            result.Add(request.Headers.ToString().TrimEnd());
            result.Add("Host: " + request.RequestUri.Host);

            Trace(string.Join(Environment.NewLine, result));
        }

        public static void TraceResponse(this HttpWebResponse response)
        {
            var result = new List<string>();

            result.Add(string.Format(CultureInfo.InvariantCulture, "StatusCode={0}, StatusDescription={1}", (int)response.StatusCode, response.StatusDescription));
            result.Add(response.Headers.ToString().TrimEnd());

            Trace(string.Join(Environment.NewLine, result));
        }

        #endregion

        private static void Trace(string data)
        {
            Debug.WriteLine(string.Empty);
            Debug.WriteLine(data);
            Debug.WriteLine(string.Empty);
        }
    }
}
