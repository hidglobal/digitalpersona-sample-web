using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Live question information to enrollment operation.
    /// </summary>
    public class LiveQuestionEnrollmentData : DataObject
    {
        /// <summary>
        /// Collection of <see cref="LiveQuestionAnswer"/>.
        /// </summary>
        public IEnumerable<LiveQuestionAnswer> LiveEnrollment { get; private set; }

        /// <summary>
        /// Initialize new instance of <see cref="LiveQuestionEnrollmentData"/> with specific enrollment information.
        /// </summary>
        /// <param name="liveEnrollment">Collection of <see cref="LiveQuestionAnswer"/>.</param>
        public LiveQuestionEnrollmentData(IEnumerable<LiveQuestionAnswer> liveEnrollment)
        {
            if (liveEnrollment == null) 
                throw new ArgumentNullException("liveEnrollment");

            LiveEnrollment = liveEnrollment.ToArray();

            byte customNumber = 101; //we need some magic to enumerate custom questions.
            foreach (var item in LiveEnrollment.Where(p => p.Question.Number > 100 || p.Question.Type == LiveQuestionType.Custom))
            {
                item.Question.Number = customNumber;
                item.Answer.Number = customNumber;
                customNumber++;
            }
        }
    }
}
