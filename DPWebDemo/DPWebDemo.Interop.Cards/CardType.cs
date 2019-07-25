using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Interop.Cards
{
    /// <summary>
    /// Card type. (Proximity, smart etc.)
    /// </summary>
    public enum CardType : uint
    {
        None = 0,
        
        Smartcard = 1,
        
        Contactless = 2,
        
        /// <summary>
        /// A proximity card or prox card is a "contactless" smart card which can be read without inserting it into a reader device, 
        /// as required by earlier magnetic stripe cards such as credit cards and "contact" type smart cards.
        /// </summary>
        Proximity = 4
    }
}
