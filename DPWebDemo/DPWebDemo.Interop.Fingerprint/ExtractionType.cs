using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPWebDemo.Interop.Fingerprint
{
    /// <summary>
    /// Fingerprint extraction type. 
    /// </summary>
    public enum ExtractionType
    {
        /// <summary>
        /// Fingerprint feature set to be used for enrollment.
        /// </summary>
        FeatureSetForEnrollment = 0,

        /// <summary>
        /// Fingerprint template.
        /// </summary>
        TemplateForEnrollment = 1,

        /// <summary>
        /// Fingerprint feature set to be used for verification.
        /// </summary>
        FeatureSetForAuthentication = 2,

        // FD_IMAGE_COLOR = 3
    }
}
