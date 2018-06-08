using System;
using System.Collections.Generic;
using System.Collections;

namespace Cryoprison
{
    public interface IJailbreakDetector
    {
        void Refresh();

        bool IsJailbroken { get; }

        IEnumerable<string> Violations { get; }
    }
}
