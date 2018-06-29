using System;
using System.Collections.Generic;
using System.Collections;

namespace Cryoprison
{
    /// <summary>
    /// The actual public, platform independent, interface for jailbreak detectors.
    /// </summary>
    public interface IJailbreakDetector
    {
        /// <summary>
        /// Refresh the detector, causing it to re-run detection logic the
        /// next time it is asked whether the device is jailbroken or not, or
        /// what the violations are.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Determines if the device is jailbroken or not, running the jailbreak
        /// detection code if neccessary.  The results are cached.
        /// </summary>
        bool IsJailbroken { get; }

        /// <summary>
        /// Gets the violations, assuming that the device is jailbroken, as
        /// check IDs.  Runs the jailbreak detection code if neccessary, and
        /// caches the results.
        /// </summary>
        IEnumerable<string> Violations { get; }
    }
}
