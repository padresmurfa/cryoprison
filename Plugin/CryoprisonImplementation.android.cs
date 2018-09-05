using System;
using System.Collections.Generic;
using System.Text;

namespace Cryoprison
{
    /// <summary>
    /// Interface for $safeprojectgroupname$
    /// </summary>
    public class CryoprisonImplementation : ICryoprison
    {
        public Cryoprison.Ex.Env CreateEnvironment()
        {
            return new Ex.Env();
        }

        public IJailbreakDetector CreateJailbreakDetector(Ex.Env env = null, bool? simulatorFriendly = null)
        {
            env = env ?? CreateEnvironment();

            return new Cryoprison.Android.JailbreakDetector(env, simulatorFriendly: simulatorFriendly);
        }
    }
}
