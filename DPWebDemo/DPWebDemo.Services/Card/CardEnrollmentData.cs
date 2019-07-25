using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Information to Card enrollment.
    /// </summary>
    public class CardEnrollmentData
    {
        /// <summary>
        /// Card data ID.
        /// </summary>
        public byte[] CardId { get; private set; }

        /// <summary>
        /// Initialize new instance of <see cref="CardEnrollmentData"/> with specific card id value.
        /// </summary>
        /// <param name="cardId">Card id value.</param>
        public CardEnrollmentData(byte[] cardId)
        {
            if (cardId == null)
                throw new ArgumentNullException("cardId");

            CardId = cardId;
        }
    }
}
