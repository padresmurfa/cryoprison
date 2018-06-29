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
        /// The jailbreaks that were detected.
        /// </summary>
        private List<string> jailbreaks = null;

        /// <summary>
        /// The inspectors that we use to detect jailbreaks.
        /// </summary>
        private List<IInspector> inspectors = new List<IInspector>();

        /// <summary>
        /// Call this method to add inspectors during construction.
        /// </summary>
        /// <param name="inspectors">The inspectors.</param>
        protected void AddInspectors(IEnumerable<IInspector> inspectors)
        {
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
                    if (this.jailbreaks == null)
                    {
                        var jailbreaks = this.inspectors.Select(Inspect).Where(x => x != null);

                        this.jailbreaks = jailbreaks.ToList();
                    }
                    return this.jailbreaks;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsJailbroken
        {
            get
            {
                return this.Violations.Any();
            }
        }

        /// <summary>
        /// Internal list of inspector types that are base classes and thus should
        /// not be instantiated directly.
        /// </summary>
        private static Type[] nopes = new [] { typeof(UrlNotOpenable), typeof(FileNotPresent), typeof(DirectoryNotPresent), typeof(FileNotAccessible), typeof(FileNotDestructivelyWritable),  typeof(IInspector), typeof(InspectorBase) };

        /// <summary>
        /// Instantiates the specific inspector type.  Returns null if its not
        /// to be used directly, e.g. has no default constructor or is a known
        /// base class.
        /// </summary>
        /// <returns>The instantiated inspector.</returns>
        /// <param name="type">The inspector type to instantiate.</param>
        public static IInspector Instantiate(Type type)
        {
            if (nopes.Any(x => x == type))
            {
                return null;
            }

            try
            {
                return (IInspector)Activator.CreateInstance(type);
            }
            catch (MissingMethodException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Reporter.ReportException("IInspector.Instantiate failed", ex);

                return null;
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
    }
}
