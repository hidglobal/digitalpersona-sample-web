using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Interop.Cards
{
    /// <summary>
    /// Card information.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Card name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Card type.
        /// </summary>
        public CardType CardType { get; internal set; }

        /// <summary>
        /// Token card id.
        /// </summary>
        public byte[] Id { get; internal set; }
    }
}
