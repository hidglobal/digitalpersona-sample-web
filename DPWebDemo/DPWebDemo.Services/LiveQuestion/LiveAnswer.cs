using System.Runtime.Serialization;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Answer to LiveQuestion
    /// </summary>
    public class LiveAnswer : DataObject
    {
        /// <summary>
        /// Version of Live Question class. Always 1.
        /// </summary>
        [DataMember(Name = "version")]
        public byte Version { get; private set; }

        /// <summary>
        /// Live Question number.
        /// This number must correspond to question number was received in GetEnrollmentDataResult.
        /// </summary>
        [DataMember(Name = "number")]
        public byte Number { get; internal set; }

        /// <summary>
        /// Answer text.
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Initialize new instance of <see cref="LiveAnswer"/> with specific number value.
        /// </summary>
        /// <param name="number">Answer number.</param>
        public LiveAnswer(byte number)
        {
            Number = number;
            Version = 1;
        }
    }
}
