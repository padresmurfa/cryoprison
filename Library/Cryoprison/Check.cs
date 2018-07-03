using System;

namespace Cryoprison
{
    /// <summary>
    /// A check is an id-value pair, identifying both the jailbreak and the
    /// variant which is basically the parameter sent to the inspector.
    /// </summary>
    public class Check
    {
        /// <summary>
        /// The identifier of the check, which will be used to create the
        /// identifier / jailbreak ID.  CheckId could e.g. be FOO while the
        /// identifier / jailbreak ID could e.g. be derived to be
        /// FILE_FOO_SHOULD_BE_PRESENT.
        /// </summary>
        public string CheckId { get; set; }

        /// <summary>
        /// The value, or parameter used when instantiating the jailbreak
        /// </summary>
        public string Value { get; set; }
    }
}
