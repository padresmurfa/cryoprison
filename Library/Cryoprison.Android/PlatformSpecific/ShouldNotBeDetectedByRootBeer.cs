using System;
using System.Collections.Generic;
using Com.Scottyab.Rootbeer;
using Android.App;

namespace Cryoprison.Inspectors
{
    /// <summary>
    /// Android specific jailbreak detection that uses the Rootbeer library
    /// to check for violations.
    /// https://github.com/scottyab/rootbeer
    /// </summary>
    public class ShouldNotBeDetectedByRootbeer : IInspector
    {
        /// <summary>
        /// Initialize the inspector.  This inspector does not have any need
        /// for params really, so it ignores them.
        /// </summary>
        public IInspector Init(string id, string path)
        {
            return this;
        }

        /// <inheritdoc/>
        public string Id
        {
            get
            {
                return "SHOULD_NOT_BE_DETECTED_BY_ROOTBEER";
            }
        }

        /// <inheritdoc/>
        public bool Ok
        {
            get
            {
                return !IsRooted();
            }
        }

        /// <summary>
        /// Determines if the app has been rooted, according to Rootbeer.
        /// </summary>
        public static bool IsRooted()
        {
            try
            {
                var rootBeer = new RootBeer(context: Application.Context);

                // Rather than risking false-positives due to BusyBox on the
                // stock ROM, we do not test for it.
                return rootBeer.IsRootedWithoutBusyBoxCheck;
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"Rootbeer.IsRooted bombed", ex);
                return false;
            }
        }
    }
}
