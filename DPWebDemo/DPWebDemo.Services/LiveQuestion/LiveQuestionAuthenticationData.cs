using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Live question information to authentication operation.
    /// </summary>
    public class LiveQuestionAuthenticationData : DataObject
    {
        /// <summary>
        /// Live answers.
        /// </summary>
        public IEnumerable<LiveAnswer> LiveAnswers { get; private set; }

        /// <summary>
        /// Initialize new instance of <see cref="LiveQuestionAuthenticationData"/> with specific <see cref="LiveAnswer"/> values.
        /// </summary>
        /// <param name="liveAnswers">Collection of <see cref="LiveAnswer"/>.</param>
        public LiveQuestionAuthenticationData(IEnumerable<LiveAnswer> liveAnswers)
        {
            if (liveAnswers == null)
                throw new ArgumentNullException("liveAnswers");

            LiveAnswers = liveAnswers.ToArray();
        }
    }
}
