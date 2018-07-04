using System;
using System.Collections.Generic;
using System.Linq;
using Cryoprison.Ex;

namespace Cryoprison
{
    /// <summary>
    /// A FluentApi factory for building up a collection of checks to be performed
    /// when performing a jailbreak test.
    /// </summary>
    public class Checks : EnvDependant
    {
        public Checks(Env env)
            : base(env)
        {
        }

        /// <summary>
        /// The list of checks in this fluent factory
        /// </summary>
        private List<Check> checks { get; set; } = new List<Check>();

        /// <summary>
        /// The root paths to search.  When a check is added, a version with
        /// each root path is used.  If there is a seperator required, then the
        /// caller must either always include it in the root paths, or always
        /// include it in the added values.
        /// </summary>
        private List<string> rootPaths { get; set; }

        /// <summary>
        /// Adds the specified roots to our root paths collection.
        /// </summary>
        /// <returns>The Checks factory itself (this) for continued fluent operations</returns>
        /// <param name="roots">The roots to add</param>
        public Checks AddRoots(params string[] roots)
        {
            if (roots == null)
            {
                return this;
            }

            var validRoots = roots.Where(x => !string.IsNullOrEmpty(x));

            if (this.rootPaths == null)
            {
                this.rootPaths = new List<string>(validRoots);
            }
            else
            {
                this.rootPaths.AddRange(validRoots);
            }

            return this;
        }

        /// <summary>
        /// Add an individual check to the factory
        /// </summary>
        /// <returns>The Checks factory itself (this) for continued fluent operations</returns>
        /// <param name="checkId">The check ID.</param>
        /// <param name="val">The value, or parameter used when instantiating the inspector.</param>
        public Checks Add(string checkId, string val)
        {
            if (checkId != null && !string.IsNullOrEmpty(val))
            {
                var rootPaths = this.rootPaths ?? new List<string>(new[] { "" });

                foreach (var root in rootPaths)
                {
                    this.checks.Add(new Check { CheckId = checkId, Value = root + val });
                }
            }

            return this;
        }

        /// <summary>
        /// Add a collection of checks to the factory
        /// </summary>
        /// <returns>The Checks factory itself (this) for continued fluent operations</returns>
        /// <param name="checkId">The check ID.</param>
        /// <param name="val">The values, or parameters used when instantiating the inspectors.</param>
        public Checks Add(string checkId, params string[] vals)
        {
            if (checkId != null && vals != null)
            {
                foreach (var val in vals)
                {
                    this.Add(checkId, val);
                }
            }

            return this;
        }

        /// <summary>
        /// Gets the instantiated, initialized inspectors that have been declared
        /// via Add statements.
        /// </summary>
        public IEnumerable<IInspector> GetInspectors<T>()
            where T : IInspector, new()
        {
            var retval = new List<IInspector>();

            foreach (var check in this.checks)
            {
                T uninitialized;
                try
                {
                    uninitialized = new T();
                }
                catch (Exception ex)
                {
                    var typeName = typeof(T).FullName;
                    Env.Reporter.ReportException($"GetInspectors bombed while constructing {typeName}", ex);
                    continue;
                }

                IInspector initialized;
                try
                {
                    var checkId = check?.CheckId?.ToUpperInvariant();
                    initialized = uninitialized.Init(this.Env, checkId, check.Value);
                }
                catch (Exception ex)
                {
                    var typeName = typeof(T)?.FullName;
                    Env.Reporter.ReportException($"GetInspectors bombed while initializing {typeName}", ex);
                    continue;
                }

                retval.Add(initialized);
            }

            return retval;
        }
    }
}
