using System;
using System.Collections.Generic;
using System.Collections;

namespace Cryoprison
{
    public interface IJailbreakDetector
    {
        bool IsJailbroken { get; }

        IEnumerable<string> Violations { get; }
    }
}
