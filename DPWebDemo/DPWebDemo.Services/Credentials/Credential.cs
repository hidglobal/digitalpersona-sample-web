using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DPWebDemo.Services.Biometric;
using Newtonsoft.Json;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Class to create credentials.
    /// </summary>
    public static class Credential
    {
        /// <summary>
        /// Convert <see cref="CredentialType"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="credentialType"><see cref="CredentialType"/> to convert.</param>
        /// <returns>Corresponding <see cref="Guid"/>.</returns>
        internal static Guid ToGuid(CredentialType credentialType)
        {
            return JsonConvert.DeserializeObject<Guid>(JsonConvert.SerializeObject(credentialType));
        }

        #region Authentication

        /// <summary>
        /// Create password credential for authentication.
        /// </summary>
        /// <param name="authenticationData"><see cref="PasswordAuthenticationData"/> instance.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IAuthenticationCredential Create(PasswordAuthenticationData authenticationData)
        {
            if (authenticationData == null)
                throw new ArgumentNullException("authenticationData");

            return new AuthenticationCredential(CredentialType.Password, authenticationData.Password);
        }

        /// <summary>
        /// Create OTP credential for authentication.
        /// </summary>
        /// <param name="authenticationData"><see cref="OtpAuthenticationData"/> instance.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IAuthenticationCredential Create(OtpAuthenticationData authenticationData)
        {
            if (authenticationData == null)
                throw new ArgumentNullException("authenticationData");

            return new AuthenticationCredential(CredentialType.OneTimePassword, authenticationData.OneTimePassword);
        }

        /// <summary>
        /// Create PIN credential for authentication.
        /// </summary>
        /// <param name="authenticationData"><see cref="PinAuthenticationData"/> instance.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IAuthenticationCredential Create(PinAuthenticationData authenticationData)
        {
            if (authenticationData == null)
                throw new ArgumentNullException("authenticationData");

            return new AuthenticationCredential(CredentialType.Pin, authenticationData.Pin);
        }

        /// <summary>
        /// Create card credential for authentication.
        /// </summary>
        /// <param name="authenticationData"><see cref="CardAuthenticationData"/> instance.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IAuthenticationCredential Create(CardAuthenticationData authenticationData)
        {
            if (authenticationData == null)
                throw new ArgumentNullException("authenticationData");

            return new AuthenticationCredential(CredentialType.ProximityCard, authenticationData.CardId);
        }

        /// <summary>
        /// Create live question credential for authentication.
        /// </summary>
        /// <param name="authenticationData"><see cref="LiveQuestionAuthenticationData"/> instance.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IAuthenticationCredential Create(LiveQuestionAuthenticationData authenticationData)
        {
            if (authenticationData == null)
                throw new ArgumentNullException("authenticationData");

            return new AuthenticationCredential(CredentialType.LiveQuestions, authenticationData.LiveAnswers);
        }

        /// <summary>
        /// Create biometric credential for authentication.
        /// </summary>
        /// <param name="authenticationData"><see cref="authenticationData"/> instance.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IAuthenticationCredential Create(FingerprintAuthenticationData authenticationData)
        {
            if (authenticationData == null)
                throw new ArgumentNullException("authenticationData");

            return new AuthenticationCredential(CredentialType.Fingerprint, new[] { authenticationData.BiometricSample });
        }

        #endregion

        #region Enrollment

        /// <summary>
        /// Create PIN credential for enrolment operation.
        /// </summary>
        /// <param name="enrollmentData"><see cref="PinEnrollmentData"/> instance.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IEnrollmentCredential Create(PinEnrollmentData enrollmentData)
        {
            if (enrollmentData == null)
                throw new ArgumentNullException("enrollmentData");

            return new EnrollmentCredential(CredentialType.Pin, enrollmentData.Pin);
        }

        /// <summary>
        /// Create password credential for enrolment operation.
        /// </summary>
        /// <param name="enrollmentData">Enrollment data.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IEnrollmentCredential Create(PasswordEnrollmentData enrollmentData)
        {
            if (enrollmentData == null)
                throw new ArgumentNullException("enrollmentData");

            return new EnrollmentCredential(CredentialType.Password, enrollmentData);
        }

        /// <summary>
        /// Create fingerprint credential for enrolment operation.
        /// </summary>
        /// <param name="enrollmentData"><see cref="FingerprintEnrollmentData"/> for enrollment.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IEnrollmentCredential Create(FingerprintEnrollmentData enrollmentData)
        {
            if (enrollmentData == null)
                throw new ArgumentNullException("enrollmentData");

            return new EnrollmentCredential(CredentialType.Fingerprint, enrollmentData);
        }

        /// <summary>
        /// Create card credential for enrollment.
        /// </summary>
        /// <param name="cardId"><see cref="CardEnrollmentData"/> instance.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IEnrollmentCredential Create(CardEnrollmentData cardId)
        {
            if (cardId == null)
                throw new ArgumentNullException("cardId");

            return new EnrollmentCredential(CredentialType.ProximityCard, cardId.CardId);
        }

        /// <summary>
        /// Create OTP credential for enrolment operation.
        /// </summary>
        /// <param name="enrollmentData"><see cref="OtpEnrollmentData"/> for enrollment.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IEnrollmentCredential Create(OtpEnrollmentData enrollmentData)
        {
            if (enrollmentData == null)
                throw new ArgumentNullException("enrollmentData");

            return new EnrollmentCredential(CredentialType.OneTimePassword, enrollmentData);
        }

        /// <summary>
        /// Create Live Question credential for enrolment operation.
        /// </summary>
        /// <param name="enrollmentData"><see cref="LiveQuestionEnrollmentData"/> for enrollment.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static IEnrollmentCredential Create(LiveQuestionEnrollmentData enrollmentData)
        {
            if (enrollmentData == null)
                throw new ArgumentNullException("enrollmentData");

            return new EnrollmentCredential(CredentialType.LiveQuestions, enrollmentData.LiveEnrollment);
        }

        #endregion

        /// <summary>
        /// Create empty credential.
        /// </summary>
        /// <param name="credentialType"><see cref="CredentialType"/> to create.</param>
        /// <returns>Corresponding <see cref="ICredential"/> implemetation.</returns>
        public static ICredential CreateEmpty(CredentialType credentialType)
        {
            return new EmptyCredential(credentialType);
        }

        /// <summary>
        /// Implementation of <see cref="IAuthenticationCredential"/>.
        /// </summary>
        private class AuthenticationCredential : DataObject, IAuthenticationCredential
        {
            /// <summary>
            /// Credential type. See <see cref="CredentialType"/>.
            /// </summary>
            [DataMember(Name = "id")]
            public CredentialType Type { get; private set; }

            /// <summary>
            /// Credential data.
            /// </summary>
            [DataMember(Name = "data")]
            [JsonConverter(typeof(Base64UrlConverter))]
            public object Data { get; private set; }

            /// <summary>
            /// Initialize new instance of <see cref="AuthenticationCredential"/> using specfific credential type and data.
            /// </summary>
            public AuthenticationCredential(CredentialType credentialType, object data)
            {
                Type = credentialType;
                Data = data;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IEnrollmentCredential"/>.
        /// </summary>
        private class EnrollmentCredential : DataObject, IEnrollmentCredential
        {
            /// <summary>
            /// Credential type. See <see cref="CredentialType"/>.
            /// </summary>
            [DataMember(Name = "id")]
            public CredentialType Type { get; private set; }

            /// <summary>
            /// Credential data.
            /// </summary>
            [DataMember(Name = "data")]
            [JsonConverter(typeof(Base64UrlConverter))]
            public object Data { get; private set; }

            /// <summary>
            /// Initialize new instance of <see cref="EnrollmentCredential"/> using specfific credential type and data.
            /// </summary>
            public EnrollmentCredential(CredentialType credentialType, object data)
            {
                Type = credentialType;
                Data = data;
            }
        }

        /// <summary>
        /// Empty implementation of <see cref="ICredential"/>.
        /// </summary>
        private class EmptyCredential : DataObject, ICredential
        {
            /// <summary>
            /// Credential type. See <see cref="CredentialType"/>.
            /// </summary>
            [DataMember(Name = "id")]
            public CredentialType Type { get; private set; }

            /// <summary>
            /// Credential data.
            /// </summary>
            [IgnoreDataMember]
            public object Data { get { return null; } }

            /// <summary>
            /// Initialize new instance of <see cref="EmptyCredential"/> using specfific credential type and data.
            /// </summary>
            public EmptyCredential(CredentialType credentialType)
            {
                Type = credentialType;
            }
        }
    }
}
