using System.Runtime.Serialization;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Type of Live Question.
    /// </summary>
    [DataContract]
    public enum LiveQuestionType
    {
        /// <summary>
        /// Regular Question.
        /// </summary>
        [EnumMember]
        Regular = 0,

        /// <summary>
        /// Custom Question.
        /// </summary>
        [EnumMember]
        Custom = 1,

    }
}
