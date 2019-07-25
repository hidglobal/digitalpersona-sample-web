using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DPWebDemo.Services.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Service to manage user credentials.
    /// </summary>
    public class EnrollmentService : EnrollmentInformationService
    {
        /// <summary>
        /// Get full service address.
        /// </summary>
        /// <example>
        /// "https://example.com/DPWebAUTH/ServiceName.svc"
        /// </example>
        /// <returns>Full authentication service address string.</returns>
        protected override Uri GetRequestUri(string pathAppendix)
        {
            if (pathAppendix == null)
                throw new ArgumentNullException("pathAppendix");

            return new Uri("https://" + ServerName + "/DPWebEnroll/DPWebEnrollService.svc" + pathAppendix);
        }

        /// <summary>
        /// CreateUser method creates user account in Altus database.
        /// </summary>
        /// <param name="officerTicket">JSON Web Token of Security Officer.</param>
        /// <param name="user">User account needs to be created.</param>
        /// <param name="password">String which represents initial password for newly created user account. </param>
        /// <remarks>
        /// This method makes sense only if Altus is used as backend server. 
        /// In Altus AD user account is created in Active Directory and Administrator must use standard Active Directory tools to create it.
        /// 
        /// Security Officer should use DigitalPersona Web AUTH Service to authenticate himself and acquire this token.
        /// Token must be valid to call succeeded. To be valid token must be:
        ///  1) issued no longer than 10 minutes before the operation,
        ///  2) one of the Primary credentials must be used to acquire this token and 
        ///  3) token owner must have a rights to create user account in Altus (AD LDS) database.
        /// 
        /// We cannot create user account without setting initial. 
        /// Password must satisfy password complexity policy set for AD LDS database otherwise call will fail.
        /// </remarks>
        public async Task CreateUser(Ticket officerTicket, UserName user, string password)
        {
            if (officerTicket == null)
                throw new ArgumentNullException("officerTicket");

            if (user == null)
                throw new ArgumentNullException("user");

            if (password == null)
                throw new ArgumentNullException("password");

            try
            {
                var request = HttpHelper.CreateHttpRequest(GetRequestUri("/CreateUser"), RequestMethod.Put);

                var postParameters = new List<BodyParameter>
                    {
                        new BodyParameter("secOfficer", officerTicket),
                        new BodyParameter("user", user),
                        new BodyParameter("password", password)
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
        /// DeleteUser method deletes user account from Altus database. 
        /// </summary>
        /// <param name="officerTicket">JSON Web Token of Security Officer.</param>
        /// <param name="user">User account needs to be deleted.</param>
        /// <remarks>
        /// This method makes sense only if Altus is used as backend server.
        /// In Altus AD user account is deleted in Active Directory and Administrator should use standard Active Directory tools to do so.
        /// 
        /// Security Officer should use DigitalPersona Web AUTH Service to authenticate itself and acquire this token.
        /// Token must be valid to call succeeded. To be valid token must be:
        /// 1) issued no longer than 10 minutes before the operation,
        /// 2) one of the Primary credentials must be used to acquire this token and
        /// 3) token owner must have a rights to delete user account in Altus (AD LDS) database.
        /// </remarks>
        public async Task DeleteUser(Ticket officerTicket, UserName user)
        {
            if (officerTicket == null)
                throw new ArgumentNullException("officerTicket");

            if (user == null)
                throw new ArgumentNullException("user");

            try
            {
                var request = HttpHelper.CreateHttpRequest(GetRequestUri("/DeleteUser"), RequestMethod.Delete);

                var postParameters = new List<BodyParameter>
                    {
                        new BodyParameter("secOfficer", officerTicket),
                        new BodyParameter("user", user)
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
        /// EnrollUserCredentials method enrolls (or re-enrolls) specific credentials for specific user and store credential data in Altus (AD) database.
        /// This method will work for both Altus and Altus AD backend servers.
        /// </summary>
        /// <param name="owner">JSON Web Token of the owner of credentials.</param>
        /// <param name="credential">Credential to be enrolled</param>
        /// <remarks>
        /// User should use DigitalPersona Web AUTH Service to authenticate itself and acquire this token.
        /// Token must be valid to call succeeded. To be valid token must be:
        /// 1) issued no longer than 10 minutes before the operation,
        /// 2) one of the Primary credentials (or same credentials) must be used to acquire this token.
        /// 3) user should have rights to enroll himself
        /// </remarks> 
        public Task EnrollCredential(Ticket owner, IEnrollmentCredential credential)
        {
            return EnrollCredentialCore(null, owner, credential);
        }

        /// <summary>
        /// EnrollUserCredentials method enrolls (or re-enrolls) specific credentials for specific user and store credential data in Altus (AD) database.
        /// This method will work for both Altus and Altus AD backend servers.
        /// </summary>
        /// <param name="officerTicket">JSON Web Token of Security Officer.</param>
        /// <param name="owner">JSON Web Token of the owner of credentials.</param>
        /// <param name="credential">Credential to be enrolled</param>
        /// <remarks>
        /// Security Officer should use DigitalPersona Web AUTH Service to authenticate itself and acquire this token. 
        /// Token must be valid to call succeeded. To be valid token must be: 
        /// 1) issued no longer than 10 minutes before the operation,
        /// 2) one of the Primary credentials must be used to acquire this token and 
        /// 3) token owner must have a rights to enroll user in Altus (AD LDS) or Altus AD (Active Directory) database.
        /// 
        /// User should use DigitalPersona Web AUTH Service to authenticate itself and acquire this token.
        /// Token must be valid to call succeeded. To be valid token must be:
        /// 1) issued no longer than 10 minutes before the operation,
        /// 2) one of the Primary credentials (or same credentials) must be used to acquire this token.
        /// </remarks> 
        public async Task EnrollCredential(Ticket officerTicket, Ticket owner, IEnrollmentCredential credential)
        {
            /*if (officerTicket == null)
                throw new ArgumentNullException("officerTicket"); */

            if (owner == null)
                throw new ArgumentNullException("owner");

            if (credential == null)
                throw new ArgumentNullException("credential");

            await EnrollCredentialCore(officerTicket, owner, credential);
        }

        private async Task EnrollCredentialCore(Ticket officerTicket, Ticket owner, IEnrollmentCredential credential)
        {
            try
            {
                var request = HttpHelper.CreateHttpRequest(GetRequestUri("/EnrollUserCredentials"), RequestMethod.Put);

                var postParameters = new List<BodyParameter>
                    {
                        new BodyParameter("secOfficer", officerTicket),
                        new BodyParameter("owner", owner),
                        new BodyParameter("credential", credential)
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
        /// This method deletes (un-enrolls) specific credentials for specific user and remove credential data from Altus (DA) database.
        /// </summary>
        /// <param name="owner">JSON Web Token of the owner of credentials. </param>
        /// <param name="credential">Credential to be deleted. </param>
        /// <remarks>
        /// DeleteUserCredentials method deletes (un-enrolls) specific credentials for specific user and remove credential data from Altus (DA) database. 
        /// This method will work for both Altus and Altus AD backend servers.
        /// User should use DigitalPersona Web AUTH Service to authenticate itself and acquire this token. 
        /// Token must be valid to call succeeded. To be valid token must be:
        /// 1) issued no longer than 10 minutes before the operation,
        /// 2) one of the Primary credentials (or same credentials) must be used to acquire this token.
        /// 3) user should have rights to enroll himself
        /// </remarks>
        public Task DeleteCredential(Ticket owner, ICredential credential)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            if (credential == null)
                throw new ArgumentNullException("credential");

            return DeleteCredential(null, owner, credential);
        }

        /// <summary>
        /// This method deletes (un-enrolls) specific credentials for specific user and remove credential data from Altus (DA) database.
        /// </summary>
        /// <param name="officerTicket">JSON Web Token of Security Officer.</param>
        /// <param name="owner">JSON Web Token of the owner of credentials. </param>
        /// <param name="credential">Credential to be deleted. </param>
        /// <remarks>
        /// DeleteUserCredentials method deletes (un-enrolls) specific credentials for specific user and remove credential data from Altus (DA) database. 
        /// This method will work for both Altus and Altus AD backend servers.
        /// 
        /// Security Officer should use DigitalPersona Web AUTH Service to authenticate itself and acquire this token.
        /// Token must be valid to call succeed. To be valid token must be: 
        /// 1) issued no longer than 10 minutes before the operation, 
        /// 2) one of the Primary credentials must be used to acquire this token and 
        /// 3) token owner must have a rights to enroll user in Altus (AD LDS) or Altus AD (Active Directory) database.
        ///  NOTE: This parameter is optional. If user has rights to enroll himself (self-enrollment allowed), caller may provide "null" to this parameter.
        /// 
        /// User should use DigitalPersona Web AUTH Service to authenticate itself and acquire this token. 
        /// Token must be valid to call succeeded. To be valid token must be:
        /// 1) issued no longer than 10 minutes before the operation,
        /// 2) one of the Primary credentials (or same credentials) must be used to acquire this token.
        /// </remarks>
        public async Task DeleteCredential(Ticket officerTicket, Ticket owner, ICredential credential)
        {
            if (officerTicket == null)
                throw new ArgumentNullException("officerTicket");

            if (owner == null)
                throw new ArgumentNullException("owner");

            if (credential == null)
                throw new ArgumentNullException("credential");

            await DeleteCredentialCore(officerTicket, owner, credential);
        }

        private async Task DeleteCredentialCore(Ticket officerTicket, Ticket owner, ICredential credential)
        {
            try
            {
                var request = HttpHelper.CreateHttpRequest(GetRequestUri("/DeleteUserCredentials"), RequestMethod.Delete);

                var postParameters = new List<BodyParameter>
                    {
                        new BodyParameter("secOfficer", officerTicket),
                        new BodyParameter("owner", owner),
                        new BodyParameter("credential", credential)
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
        /// Get some public (biographic) information about user, like user surname, date of birth, e-mail address, etc.
        /// </summary>
        /// <typeparam name="T">Type of requested attribute. Should be <see cref="T:byte[]"/>, <see cref="string"/>, <see cref="bool"/> or <see cref="int"/>.
        /// </typeparam>
        /// <param name="ticket">JSON Web Token of user who requests this information.</param>
        /// <param name="user">User which information requested.</param>
        /// <param name="attributeName">Name of the information requested. 
        /// Both AD and AD LDS are LDAP databases so this name must be Ldap-Display-Name of Attribute Schema in User object in LDAP database.</param>
        /// <returns>Collection of attribute value.</returns>
        /// <remarks>
        /// JSON Web Token of user who requests this information.
        ///  This could be an attribute owner, Security Officer, Administrator or any user who has rights to read this information.
        ///  Token must be valid to call succeed. To be valid token must be:
        ///  1) issued no longer than 10 minutes before the operation,
        ///  2) one of the Primary credentials must be used to acquire this token and
        ///  3) token owner must have a rights to read this attribute user in Altus (AD LDS) or Altus AD (Active Directory) database.
        /// </remarks>
        public async Task<T> GetSingleValueUserAttribute<T>(Ticket ticket, UserName user, string attributeName)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (user == null)
                throw new ArgumentNullException("user");

            if (attributeName == null)
                throw new ArgumentNullException("attributeName");


            var value = await GetMultiValueUserAttribute<T>(ticket, user, attributeName);
            if (value.Count() > 1)
                throw new InvalidOperationException("Attribute is multivalue. Use GetMultiValueUserAttribute");
            return value.SingleOrDefault();
        }

        /// <summary>
        /// Get some public (biographic) information about user from multivalue attribute.
        /// </summary>
        /// <typeparam name="T">Type of requested attribute. Should be <see cref="T:byte[]"/>, <see cref="string"/>, <see cref="bool"/> or <see cref="int"/>.
        /// </typeparam>
        /// <param name="ticket">JSON Web Token of user who requests this information.</param>
        /// <param name="user">User which information requested.</param>
        /// <param name="attributeName">Name of the information requested. 
        /// Both AD and AD LDS are LDAP databases so this name must be Ldap-Display-Name of Attribute Schema in User object in LDAP database.</param>
        /// <returns>Collection of attribute value.</returns>
        /// <remarks>
        /// JSON Web Token of user who requests this information.
        ///  This could be an attribute owner, Security Officer, Administrator or any user who has rights to read this information.
        ///  Token must be valid to call succeed. To be valid token must be:
        ///  1) issued no longer than 10 minutes before the operation,
        ///  2) one of the Primary credentials must be used to acquire this token and
        ///  3) token owner must have a rights to read this attribute user in Altus (AD LDS) or Altus AD (Active Directory) database.
        /// </remarks>
        public async Task<IEnumerable<T>> GetMultiValueUserAttribute<T>(Ticket ticket, UserName user, string attributeName)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (user == null)
                throw new ArgumentNullException("user");

            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            try
            {
                var request = HttpHelper.CreateHttpRequest(GetRequestUri("/GetUserAttribute"), RequestMethod.Post);

                var postParameters = new List<BodyParameter>
                    {
                        new BodyParameter("ticket", ticket),
                        new BodyParameter("user", user),
                        new BodyParameter("attributeName", attributeName)
                    };

                await request.WritePostParameters(postParameters.ToArray());

                var answer = await request.GetResponseData();
                var jobj = JObject.Parse(answer)["GetUserAttributeResult"];

                var userAttribute = UserAttribute.ParseValues<T>(jobj.ToString());

                return userAttribute;

            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }

        /// <summary>
        /// Update specific public data (attribute) for specific user.
        /// </summary>
        /// <typeparam name="T">Type of requested attribute. Should be <see cref="T:byte[]"/>, <see cref="string"/>, <see cref="bool"/> or <see cref="int"/>.
        /// </typeparam>
        /// <param name="ticket">JSON Web Token of user who requests attribute modification.</param>
        /// <param name="user">User which attribute needs to be modified.</param>
        /// <param name="attributeName">Name of the information requested. Both AD and AD LDS are LDAP databases so this name must be Ldap-Display-Name of Attribute Schema in User object in LDAP database.</param>
        /// <param name="value">Data needs to be written.</param>
        /// <returns></returns>
        /// <remarks>This method makes sense only for Altus as backend server (AD LDS), for Active Directory Administrator must use standard AD tools to manage attributes (with exception of DP specific attributes).
        /// 
        /// JSON Web Token of user who requests attribute modification. 
        /// This could be an attribute owner, Security Officer, Administrator or any user who has rights to write this information.
        /// Token must be valid to call succeed. To be valid token must be: 
        ///  1) issued no longer than 10 minutes before the operation,
        ///  2) one of the Primary credentials must be used to acquire this token and
        ///  3) token owner must have a rights to write this attribute user in Altus (AD LDS) or Altus AD (Active Directory) database.
        /// </remarks>
        public async Task UpdateSingleValueUserAttribute<T>(Ticket ticket, UserName user, string attributeName, T value)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (user == null)
                throw new ArgumentNullException("user");

            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            if (value == null)
                throw new ArgumentNullException("value");

            await PerformAttributeAction(ticket, user, attributeName, value, UserAttributeAction.Update);
        }

        /// <summary>
        /// Update specific public data (attribute) for specific user.
        /// </summary>
        /// <typeparam name="T">Type of requested attribute. Should be <see cref="T:byte[]"/>, <see cref="string"/>, <see cref="bool"/> or <see cref="int"/>.
        /// </typeparam>
        /// <param name="ticket">JSON Web Token of user who requests attribute modification.</param>
        /// <param name="user">User which attribute needs to be modified.</param>
        /// <param name="attributeName">Name of the information requested. Both AD and AD LDS are LDAP databases so this name must be Ldap-Display-Name of Attribute Schema in User object in LDAP database.</param>
        /// <param name="value">Data needs to be written.</param>
        /// <returns></returns>
        /// <remarks>This method makes sense only for Altus as backend server (AD LDS), for Active Directory Administrator must use standard AD tools to manage attributes (with exception of DP specific attributes).
        /// 
        /// JSON Web Token of user who requests attribute modification. 
        /// This could be an attribute owner, Security Officer, Administrator or any user who has rights to write this information.
        /// Token must be valid to call succeed. To be valid token must be: 
        ///  1) issued no longer than 10 minutes before the operation,
        ///  2) one of the Primary credentials must be used to acquire this token and
        ///  3) token owner must have a rights to write this attribute user in Altus (AD LDS) or Altus AD (Active Directory) database.
        /// </remarks>
        public async Task UpdateMultiValueUserAttribute<T>(Ticket ticket, UserName user, string attributeName, IEnumerable<T> value)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (user == null)
                throw new ArgumentNullException("user");

            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            if (value == null)
                throw new ArgumentNullException("value");

            await PerformAttributeAction(ticket, user, attributeName, value, UserAttributeAction.Update);
        }

        /// <summary>
        /// Clear specific public data (attribute) for specific user.
        /// </summary>
        /// <typeparam name="T">Type of requested attribute. Should be <see cref="T:byte[]"/>, <see cref="string"/>, <see cref="bool"/> or <see cref="int"/>.
        /// </typeparam>
        /// <param name="ticket">JSON Web Token of user who requests attribute modification.</param>
        /// <param name="user">User which attribute needs to be modified.</param>
        /// <param name="attributeName">Name of the information requested. Both AD and AD LDS are LDAP databases so this name must be Ldap-Display-Name of Attribute Schema in User object in LDAP database.</param>
        /// <param name="value">Data needs to be written.</param>
        /// <returns></returns>
        /// <remarks>This method makes sense only for Altus as backend server (AD LDS), for Active Directory Administrator must use standard AD tools to manage attributes (with exception of DP specific attributes).
        /// 
        /// JSON Web Token of user who requests attribute modification. 
        /// This could be an attribute owner, Security Officer, Administrator or any user who has rights to write this information.
        /// Token must be valid to call succeed. To be valid token must be: 
        ///  1) issued no longer than 10 minutes before the operation,
        ///  2) one of the Primary credentials must be used to acquire this token and
        ///  3) token owner must have a rights to write this attribute user in Altus (AD LDS) or Altus AD (Active Directory) database.
        /// </remarks>
        public async Task ClearUserAttribute(Ticket ticket, UserName user, string attributeName)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (user == null)
                throw new ArgumentNullException("user");

            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            await PerformAttributeAction(ticket, user, attributeName, null, UserAttributeAction.Clear);
        }

        /// <summary>
        /// Append specific public data to multivalue attribute for specific user.
        /// </summary>
        /// <typeparam name="T">Type of requested attribute. Should be <see cref="T:byte[]"/>, <see cref="string"/>, <see cref="bool"/> or <see cref="int"/>.
        /// </typeparam>
        /// <param name="ticket">JSON Web Token of user who requests attribute modification.</param>
        /// <param name="user">User which attribute needs to be modified.</param>
        /// <param name="attributeName">Name of the information requested. Both AD and AD LDS are LDAP databases so this name must be Ldap-Display-Name of Attribute Schema in User object in LDAP database.</param>
        /// <param name="value">Data needs to be written.</param>
        /// <returns></returns>
        /// <remarks>This method makes sense only for Altus as backend server (AD LDS), for Active Directory Administrator must use standard AD tools to manage attributes (with exception of DP specific attributes).
        /// 
        /// JSON Web Token of user who requests attribute modification. 
        /// This could be an attribute owner, Security Officer, Administrator or any user who has rights to write this information.
        /// Token must be valid to call succeed. To be valid token must be: 
        ///  1) issued no longer than 10 minutes before the operation,
        ///  2) one of the Primary credentials must be used to acquire this token and
        ///  3) token owner must have a rights to write this attribute user in Altus (AD LDS) or Altus AD (Active Directory) database.
        /// </remarks>
        public async Task AppendMultiValueUserAttribute<T>(Ticket ticket, UserName user, string attributeName, IEnumerable<T> value)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (user == null)
                throw new ArgumentNullException("user");

            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            if (value == null)
                throw new ArgumentNullException("value");

            await PerformAttributeAction(ticket, user, attributeName, value, UserAttributeAction.Append);
        }

        /// <summary>
        /// Delete specific public data to multivalue attribute for specific user.
        /// </summary>
        /// <typeparam name="T">Type of requested attribute. Should be <see cref="T:byte[]"/>, <see cref="string"/>, <see cref="bool"/> or <see cref="int"/>.
        /// </typeparam>
        /// <param name="ticket">JSON Web Token of user who requests attribute modification.</param>
        /// <param name="user">User which attribute needs to be modified.</param>
        /// <param name="attributeName">Name of the information requested. Both AD and AD LDS are LDAP databases so this name must be Ldap-Display-Name of Attribute Schema in User object in LDAP database.</param>
        /// <param name="value">Data needs to be written.</param>
        /// <returns></returns>
        /// <remarks>This method makes sense only for Altus as backend server (AD LDS), for Active Directory Administrator must use standard AD tools to manage attributes (with exception of DP specific attributes).
        /// 
        /// JSON Web Token of user who requests attribute modification. 
        /// This could be an attribute owner, Security Officer, Administrator or any user who has rights to write this information.
        /// Token must be valid to call succeed. To be valid token must be: 
        ///  1) issued no longer than 10 minutes before the operation,
        ///  2) one of the Primary credentials must be used to acquire this token and
        ///  3) token owner must have a rights to write this attribute user in Altus (AD LDS) or Altus AD (Active Directory) database.
        /// </remarks>
        public async Task DeleteMultiValueUserAttribute<T>(Ticket ticket, UserName user, string attributeName, IEnumerable<T> value)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (user == null)
                throw new ArgumentNullException("user");

            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            if (value == null)
                throw new ArgumentNullException("value");

            await PerformAttributeAction(ticket, user, attributeName, value, UserAttributeAction.Delete);
        }

        private async Task PerformAttributeAction(Ticket ticket, UserName user, string attributeName, object value, UserAttributeAction action)
        {
            try
            {
                var request = HttpHelper.CreateHttpRequest(GetRequestUri("/PutUserAttribute"), RequestMethod.Put);

                var postParameters = new List<BodyParameter>
                    {
                        new BodyParameter("ticket", ticket),
                        new BodyParameter("user", user),
                        new BodyParameter("attributeName", attributeName),
                        new BodyParameter("action", (int)action),
                        new BodyParameter("attributeData", UserAttribute.Create(value))
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
