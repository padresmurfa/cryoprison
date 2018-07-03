using System;
using Cryoprison;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cryoprison.Inspectors;

namespace Cryoprison
{
    /// <summary>
    /// Base class for platform specific jailbreak detectors
    /// </summary>
    public class JailbreakDetector : IJailbreakDetector
    {
        /// <summary>
        /// The jailbreaks that were detected, or null if we need to run the
        /// inspection process first.
        /// </summary>
        protected List<string> jailbreaks = null;

        /// <summary>
        /// The inspectors that we use to detect jailbreaks.
        /// </summary>
        protected List<IInspector> inspectors = new List<IInspector>();

        /// <summary>
        /// Call this method to add inspectors during construction.
        /// </summary>
        /// <param name="inspectors">The inspectors.</param>
        protected void AddInspectors(IEnumerable<IInspector> inspectors)
        {
            if (inspectors == null)
            {
                return;
            }

            inspectors = inspectors.Where(x => x != null);
            lock (this)
            {
                this.inspectors.AddRange(inspectors);
            }
        }

        /// <inheritdoc/>
        public void Refresh()
        {
            lock (this)
            {
                this.jailbreaks = null;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Violations
        {
            get
            {
                lock (this)
                {
                    this.RunIfNeeded();

                    var retval = new List<string>();
                    retval.AddRange(this.jailbreaks);
                    return retval;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsJailbroken
        {
            get
            {
                lock (this)
                {
                    this.RunIfNeeded();

                    return this.jailbreaks.Any();
                }
            }
        }

        /// <summary>
        /// Executes the specific inspector, wrapped with all the right reporting
        /// code, to determine if the device is jailbroken in the fashion
        /// that the inspector knows.
        /// </summary>
        /// <returns>The jailbreak ID, or null if the inspector does not believe
        /// that the system is jailbroken.</returns>
        /// <param name="inspector">The inspector.</param>
        private static string Inspect(IInspector inspector)
        {
            var id = inspector.Id;

            try
            {
                if (inspector.Ok)
                {
                    return null;   
                }

                Reporter.ReportJailbreak(id);

                return id;
            }
            catch (Exception ex)
            {
                Reporter.ReportException("Inspector crashed: " + id, ex);

                return null;
            }
        }

        /// <summary>
        /// Initializines the jailbreaks list by running the spectors, if neccessary.
        /// Must be called under lock.
        /// </summary>
        private void RunIfNeeded()
        {
            if (this.jailbreaks == null)
            {
                var jailbreaks = this.inspectors.Select(Inspect).Where(x => x != null);

                this.jailbreaks = jailbreaks.ToList();
            }
        }
    }
}
