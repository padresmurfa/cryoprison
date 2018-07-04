using System;
using Cryoprison.Ex;

namespace Cryoprison.Inspectors
{
    public abstract class InspectorBase : EnvDependant, IInspector
    {
        /// <summary>
        /// Format string for the Id string, with a single {0} for the checkId
        /// </summary>
        private string idFormat;

        /// <summary>
        /// The checkId, which is a subcomponent of the actual id
        /// </summary>
        protected string checkId;

        /// <summary>
        /// The actual ID.
        /// </summary>
        protected string id;

        /// <summary>
        /// The value, used to perform the actual check, that is a directory
        /// path.
        /// </summary>
        protected string val;

        /// <summary>
        /// Base class constructor, uses the <paramref name="idFormat"/> format string
        /// (which should have a single {0} field), to create the ID.
        /// </summary>
        /// <param name="idFormat">The format string for ids.</param>
        protected InspectorBase(string idFormat)
        {
            this.idFormat = idFormat;
        }

        /// <inheritdoc/>
        public IInspector Init(Ex.Env env, string checkId, string val)
        {
            this.Env = env;
            this.checkId = checkId;
            this.id = string.Format(this.idFormat, checkId);
            this.val = val;

            return this;
        }

        /// <inheritdoc/>
        public string Id
        {
            get
            {
                return this.id;
            }
        }

        /// <inheritdoc/>
        public abstract bool Ok { get; }

        /// <summary>
        /// Forward ReportException
        /// </summary>
        protected Action<string,Exception> ReportException => this.Env.Reporter.ReportException;

        /// <summary>
        /// Forward ReportJailbreak
        /// </summary>
        protected Action<string> ReportJailbreak => this.Env.Reporter.ReportJailbreak;
    }
}
