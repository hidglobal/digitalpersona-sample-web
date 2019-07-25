using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Service to extract secret data associated with user.
    /// </summary>
    public class SecretService : ServiceBase
    {
        /// <summary>
        /// Get full service address.
        /// </summary>
        /// <example>
        /// "https://example.com/DPWebSecrets/DPWebSecretManager.svc"
        /// </example>
        /// <returns>Full authentication service address string.</returns>
        protected override Uri GetRequestUri(string pathAppendix)
        {
            if (pathAppendix == null)
                throw new ArgumentNullException("pathAppendix");

            return new Uri("https://" + ServerName + "/DPWebSecrets/DPWebSecretManager.svc" + pathAppendix);
        }

        /// <summary>
        /// Check secret exsist on specific user.
        /// </summary>
        /// <param name="user">Secret owner.</param>
        /// <param name="secretName">Secret name.</param>
        /// <returns>Secret with specific hame des exsists.</returns>
        public async Task<bool> CheckSecretExist(UserName user, string secretName)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (secretName == null)
                throw new ArgumentNullException("secretName");

            try
            {
                var requestUrl = string.Format("/DoesSecretExist?user={0}&type={1}&secret={2}", user.Name, (int)user.Format, secretName);
                var request = HttpHelper.CreateHttpRequest(GetRequestUri(requestUrl), RequestMethod.Get);

                return await request.GetResponseData<bool>("DoesSecretExistResult");
            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }

        /// <summary>
        /// Get secret by name associated with ticket owner.
        /// </summary>
        /// <param name="ticket">JSON Web Token of user who requests secret information.</param>
        /// <param name="secretName">Secret name.</param>
        /// <returns>Secret data.</returns>
        public async Task<string> ReadSecret(Ticket ticket, string secretName)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (secretName == null)
                throw new ArgumentNullException("secretName");

            try
            {
                var request = HttpHelper.CreateHttpRequest(GetRequestUri("/ReadSecret"), RequestMethod.Post);

                await request.WritePostParameters("ticket", ticket, "secretName", secretName);

                return await request.GetResponseData<string>("ReadSecretResult", true);
            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }

        /// <summary>
        /// Write content to specific secret
        /// </summary>
        /// <param name="ticket">JSON Web Token of user who write secret information.</param>
        /// <param name="secretName">Secret name.</param>
        /// <param name="secretData">Secret data.</param>
        public async Task WriteSecret(Ticket ticket, string secretName, string secretData)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (secretName == null)
                throw new ArgumentNullException("secretName");

            if (secretData == null)
                throw new ArgumentNullException("secretData");


            try
            {

                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new Base64UrlConverter());
                var encodedData = JsonConvert.SerializeObject(secretData, settings).Trim('"');

                var request = HttpHelper.CreateHttpRequest(GetRequestUri("/WriteSecret"), RequestMethod.Put);

                var postParameters = new List<BodyParameter>
                    {
                        new BodyParameter("ticket", ticket),
                        new BodyParameter("secretName", secretName),
                        new BodyParameter("secretData", encodedData),
                    };

                await request.WritePostParameters(postParameters.ToArray());

                await request.GetResponseData();
            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }

        /// <summary>
        /// Delete specific secret from ticket owner..
        /// </summary>
        /// <param name="ticket">JSON Web Token of user who delete secret information.</param>
        /// <param name="secretName">Secret name.</param>
        public async Task DeleteSecret(Ticket ticket, string secretName)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (secretName == null)
                throw new ArgumentNullException("secretName");

            try
            {
                var request = HttpHelper.CreateHttpRequest(GetRequestUri("/DeleteSecret"), RequestMethod.Delete);

                var postParameters = new List<BodyParameter>
                    {
                        new BodyParameter("ticket", ticket),
                        new BodyParameter("secretName", secretName)
                    };

                await request.WritePostParameters(postParameters.ToArray());

                await request.GetResponseData();
            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }
    }
}
