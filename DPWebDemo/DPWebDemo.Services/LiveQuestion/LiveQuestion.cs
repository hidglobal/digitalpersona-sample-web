using System.Globalization;
using System.Runtime.Serialization;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Live question class.
    /// </summary>
    [DataContract]
    public class LiveQuestion : DataObject
    {
        /// <summary>
        /// Version of Live Question class. Should be set to 1.
        /// </summary>
        [DataMember(Name = "version")]
        public byte Version { get; private set; }

        /// <summary>
        /// Live Question number. This number has a sense only for regular questions.
        /// </summary>
        [DataMember(Name = "number")]
        public byte Number { get; internal set; }

        /// <summary>
        /// Type of Live Question.
        /// </summary>
        [DataMember(Name = "type")]
        public LiveQuestionType Type { get; private set; }

        /// <summary>
        /// Primary language id was used to display this question during enrollment.
        /// Client application should (if possible) use the same primary language id to display this question.
        /// </summary>
        [DataMember(Name = "lang_id")]
        public byte LanguageId { get; set; }

        /// <summary>
        /// Sublanguage id was used to display this question during enrollment.
        /// Client application should (if possible)  use the same sublanguage id to display this question.
        /// </summary>
        [DataMember(Name = "sublang_id")]
        public byte SublanguageId { get; set; }

        /// <summary>
        /// Keyboard layout was used for typing an answer to this question during enrollment.
        /// Client application must use same keyboard layout for user to enter the answer to this question.
        /// </summary>
        [DataMember(Name = "keyboard_layout")]
        public uint KeyboardLayoutId { get; set; }

        /// <summary>
        /// Question text.
        /// Text should be provided for both Custom as well as questions Regular and must be in appropriate language (correspond lang_id and sublang_id above).
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Initialize new instance of <see cref="LiveQuestion"/> with Type <see cref="LiveQuestionType"/> Custom.
        /// </summary>
        public LiveQuestion()
        {
            Version = 1;
            Number = 0;
            Type = LiveQuestionType.Custom;
        }

        /// <summary>
        /// Initialize new instance of <see cref="LiveQuestion"/> with specific question number value and Type <see cref="LiveQuestionType"/> Regular.
        /// </summary>
        /// <param name="number">Question number value.</param>
        public LiveQuestion(byte number)
        {
            Version = 1;
            Number = number;
            Type = LiveQuestionType.Regular;
        }
    }
}
