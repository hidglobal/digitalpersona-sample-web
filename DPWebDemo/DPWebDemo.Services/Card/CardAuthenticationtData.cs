using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Information to Card authentiaction.
    /// </summary>
    public class CardAuthenticationData
    {
        /// <summary>
        /// Card data ID.
        /// </summary>
        public byte[] CardId { get; private set; }

        /// <summary>
        /// Initialize new instance of <see cref="CardAuthenticationData"/> with specific card id value.
        /// </summary>
        /// <param name="cardId">Card id value.</param>
        public CardAuthenticationData(byte[] cardId)
        {
            if (cardId == null)
                throw new ArgumentNullException("cardId");

            CardId = cardId;
        }
    }
}
