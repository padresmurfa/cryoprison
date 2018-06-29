using System;
using System.Collections;
using System.Collections.Generic;
using Cryoprison.Inspectors;

namespace Cryoprison.Android
{
    /// <summary>
    /// Android implementation of the Jailbreak Detector.
    /// </summary>
    public class JailbreakDetector : Cryoprison.JailbreakDetector
    {
        /// <summary>
        /// Gets or sets the global jailbreak report hook, which is intended
        /// for debugging and logging.
        /// </summary>
        public static Action<string> OnJailbreakReported
        {
            get { return Cryoprison.Reporter.OnJailbreakReported; }
            set { Cryoprison.Reporter.OnJailbreakReported = value; }
        }

        /// <summary>
        /// Gets or sets the global exception report hook, which is intended
        /// for debugging and logging faults during jailbreak detection.
        /// </summary>
        /// <value>The on exception reported.</value>
        public static Action<string, Exception> OnExceptionReported
        {
            get { return Cryoprison.Reporter.OnExceptionReported; }
            set { Cryoprison.Reporter.OnExceptionReported = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.Android.JailbreakDetector"/> class.
        /// </summary>
        public JailbreakDetector()
        {

            this.AddInspectors(new[] { new ShouldNotBeDetectedByRootbeer() });
        }
    }
}
