using System;
using System.Collections.Generic;
using System.Text;

namespace Cryoprison
{
    public interface ICryoprison
    {
        Cryoprison.Ex.Env CreateEnvironment();

        IJailbreakDetector CreateJailbreakDetector(Ex.Env env = null, bool? simulatorFriendly = null);
    }
}
