using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Service to extract enrollment information about user.
    /// </summary>
    public abstract class EnrollmentInformationService : ServiceBase
    {
        /// <summary>
        /// Get credential, enrolled for specific <see cref="UserName"/>.
        /// </summary>
        /// <param name="userName">UserName to search.</param>
        /// <returns>List of enrolled <see cref="CredentialType"/>.</returns>
        public async Task<IEnumerable<CredentialType>> GetEnrolledCredentials(UserName userName)
        {
            if (userName == null)
                throw new ArgumentNullException("userName");

            try
            {
                var urlRequest = string.Format("/GetUserCredentials?user={0}&type={1}", userName.Name,
                    (ushort)userName.Format);

                var request = HttpHelper.CreateHttpRequest(GetRequestUri(urlRequest), RequestMethod.Get);

                var enrolledCreds = await request.GetResponseData<CredentialType[]>("GetUserCredentialsResult");
                return enrolledCreds;
            }
            catch (WebException ex)
            {
                throw GetServiceException(ex);
            }
        }

        /// <summary>
        /// Return fingerprint public enrollment information.
        /// </summary>
        /// <param name="userName">User which enrollment data needs to be returned. </param>
        /// <returns>Collection of enrolled fingers <see cref="FingerPosition"/>.</returns>
        public async Task<IEnumerable<FingerPosition>> GetEnrollmentDataFingerprint(UserName userName)
        {
            if (userName == null)
                throw new ArgumentNullException("userName");

            try
            {
                var credentialGuid = Credential.ToGuid(CredentialType.Fingerprint);

                var urlRequest = string.Format("/GetEnrollmentData?user={0}&type={1}&cred_id={2}", userName.Name,
                    (ushort)userName.Format, credentialGuid);

                var request = HttpHelper.CreateHttpRequest(GetRequestUri(urlRequest), RequestMethod.Get);

                var data = await request.GetResponseData<FingerprintEnrollmentData[]>("GetEnrollmentDataResult", true);

                return data.Select(p => p.Position).ToArray();
            }
            catch (WebException ex)
            {
                var serviceException = GetServiceException(ex);
                if (serviceException.HResult == -2147024288) //Not enrolled.
                    return null;
                throw serviceException;
            }
        }

        /// <summary>
        /// Return live question public enrollment information.
        /// </summary>
        /// <param name="userName">User which enrollment data needs to be returned. </param>
        /// <returns>Collection of enrolled <see cref="LiveQuestion"/>.</returns>
        public async Task<IEnumerable<LiveQuestion>> GetEnrollmentDataLiveQuestion(UserName userName)
        {
            if (userName == null)
                throw new ArgumentNullException("userName");

            try
            {
                var credentialGuid = Credential.ToGuid(CredentialType.LiveQuestions);

                var urlRequest = string.Format("/GetEnrollmentData?user={0}&type={1}&cred_id={2}", userName.Name,
                    (ushort)userName.Format, credentialGuid);

                var request = HttpHelper.CreateHttpRequest(GetRequestUri(urlRequest), RequestMethod.Get);

                var data = await request.GetResponseData<LiveQuestion[]>("GetEnrollmentDataResult", true);

                return data;
            }
            catch (WebException ex)
            {
                var serviceException = GetServiceException(ex);
                if (serviceException.HResult == -2147024288)
                    return null;
                throw serviceException;
            }
        }
    }
}