using System.Runtime.Serialization;

namespace DPWebDemo.Services
{
    /// <summary>
    /// Finger position.
    /// </summary>
    [DataContract]
    public enum FingerPosition
    {
        /// <summary>
        /// Unknown finger. Finger not specified.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Right thumb finger.
        /// </summary>
        RightThumb = 1,

        /// <summary>
        /// Right index finger.
        /// </summary>
        RightIndex = 2,

        /// <summary>
        /// Right middle finger.
        /// </summary>
        RightMiddle = 3,

        /// <summary>
        /// Right ring finger.
        /// </summary>
        RightRing = 4,

        /// <summary>
        /// Right little finger.
        /// </summary>
        RightLittle = 5,

        /// <summary>
        /// Left thumb finger.
        /// </summary>
        LeftThumb = 6,

        /// <summary>
        /// Left index finger.
        /// </summary>
        LeftIndex = 7,

        /// <summary>
        /// Left middle finger.
        /// </summary>
        LeftMiddle = 8,

        /// <summary>
        /// Left ring finger.
        /// </summary>
        LeftRing = 9,

        /// <summary>
        /// Left little finger.
        /// </summary>
        LeftLittle = 10,
    }
}
