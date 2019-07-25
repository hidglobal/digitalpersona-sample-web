using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DPWebDemo.Services.Biometric;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Service for authentication with Altus Server.
    /// </summary>
    public class AuthenticationService : EnrollmentInformationService
    {
        /// <summary>
        /// Get full authentication service address.
        /// </summary>
        /// <example>
        /// "https://example.com/DPWebAUTH/DPWebAuthService.svc"
        /// </example>
        /// <returns>Full authentication service address string.</returns>
        protected override Uri GetRequestUri(string pathAppendix)
        {
            if (pathAppendix == null) 
                throw new ArgumentNullException("pathAppendix");

            return new Uri("https://" + ServerName + "/DPWebAUTH/DPWebAuthService.svc" + pathAppendix);
        }

        /// <summary>
        /// Identify user using specific credential.
        /// </summary> 
        /// <remarks>
        /// IdentifyUser method allows identify user based on provided credential.
        /// NOTE: Not all credentials support identification for example fingerprints support identification but password not. 
        /// </remarks>
        /// <param name="credential">Credential to identification.</param>
        /// <returns>JSON ticket <see cref="Ticket"/>.</returns>
        public async Task<Ticket> Identify(IAuthenticationCredential credential)
        {
            if (credential == null)
                throw new ArgumentNullException("credential");

            try
            {
                var urlRequest = HttpHelper.CreateHttpRequest(GetRequestUri("/IdentifyUser"), RequestMethod.Post);

                await urlRequest.WritePostParameters("credential", credential);

                var ticket = await urlRequest.GetResponseData<Ticket>("IdentifyUserResult");

                return ticket;
            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }

        /// <summary>
        /// Authenticate user using provided credential.
        /// </summary>
        /// <param name="userName">User identity which needs to be authenticated.</param>
        /// <param name="credential">Credential to be used for authentication.</param>
        /// <returns>Autenticated <see cref="Ticket"/>.</returns>
        public async Task<Ticket> Authenticate(UserName userName, IAuthenticationCredential credential)
        {
            if (userName == null) 
                throw new ArgumentNullException("userName");
           
            if (credential == null)
                throw new ArgumentNullException("credential");

            try
            {
                var urlRequest = HttpHelper.CreateHttpRequest(GetRequestUri("/AuthenticateUser"),
                    RequestMethod.Post);

                await urlRequest.WritePostParameters("credential", credential, "user", userName);

                return await urlRequest.GetResponseData<Ticket>("AuthenticateUserResult");
            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }

        /// <summary>
        /// Authenticate existing ticket using provided credential.
        /// </summary>
        /// <param name="ticket">Ticket previously issued by authentication authority.</param>
        /// <param name="credential">Credential to be used for authentication.</param>
        /// <returns>Authenticated <see cref="Ticket"/>.</returns>
        public async Task<Ticket> AuthenticateTicket(Ticket ticket, IAuthenticationCredential credential)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");
           
            if (credential == null) 
                throw new ArgumentNullException("credential");

            try
            {
                var urlRequest = HttpHelper.CreateHttpRequest(GetRequestUri("/AuthenticateUserTicket"),
                    RequestMethod.Post);

                await urlRequest.WritePostParameters("credential", credential, "ticket", ticket);

                return await urlRequest.GetResponseData<Ticket>("AuthenticateUserTicketResult");
            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }
    }
}
