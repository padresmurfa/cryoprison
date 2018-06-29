using System;
using System.Collections;
using System.Collections.Generic;

namespace Cryoprison
{
    /// <summary>
    /// The public interface for inspectors, which inspect the system to determine
    /// if it is jailbroken.
    /// </summary>
    public interface IInspector
    {
        /// <summary>
        /// Gets the ID of the jailbreak and the inspector.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a value indicating whether or not this inspector instance
        /// believes that the system is jailbroken, running the test to do so.
        /// </summary>
        bool Ok { get; }

        /// <summary>
        /// Initializies the inspector with a <paramref name="checkId"/> that is
        /// combined using internal, inspector dependent logic, to form the
        /// Id, and with a value that is an extra argument
        /// that may be interpreted in inspector-dependent fashions.
        /// </summary>
        /// <returns>The initialized inspector.</returns>
        /// <param name="checkId">The check identifier.</param>
        /// <param name="val">The path.</param>
        IInspector Init(string checkId, string val);
    }
}
