using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Answer on live qustion.
    /// Note: question number must be equals answer number.
    /// </summary>
    [DataContract]
    public class LiveQuestionAnswer : DataObject
    {
        /// <summary>
        /// <see cref="LiveQuestion"/> Question information.
        /// </summary>
        [DataMember(Name = "question")]
        public LiveQuestion Question { get; private set; }

        /// <summary>
        /// <see cref="LiveAnswer"/> Answer information.
        /// </summary>
        [DataMember(Name = "answer")]
        public LiveAnswer Answer { get; private set; }

        /// <summary>
        /// Initialize new instance of <see cref="LiveQuestionAnswer"/> using specific question and answer.
        /// </summary>
        /// <param name="question"><see cref="LiveQuestion"/> instance.</param>
        /// <param name="answer"><see cref="LiveAnswer"/> instance.</param>
        public LiveQuestionAnswer(LiveQuestion question, LiveAnswer answer)
        {
            if (question == null)
                throw new ArgumentNullException("question");

            if (answer == null)
                throw new ArgumentNullException("answer");

            if (question.Number != answer.Number)
                throw new InvalidOperationException("Question number must be equals answer number.");

            Question = question;
            Answer = answer;
        }

        /// <summary>
        /// Initialize new instance of <see cref="LiveQuestionAnswer"/> using specific question and answer.
        /// </summary>
        /// <param name="question"><see cref="LiveQuestion"/> instance.</param>
        /// <param name="answer">Answer text.</param>
        public LiveQuestionAnswer(LiveQuestion question, string answer)
        {
            if (question == null)
                throw new ArgumentNullException("question");

            if (answer == null)
                throw new ArgumentNullException("answer");

            Question = question;
            Answer = new LiveAnswer(question.Number) { Text = answer };
        }
    }
}
