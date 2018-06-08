using System;
using System.Collections;
using System.Collections.Generic;

namespace Cryoprison
{
    public interface IInspector
    {
        string Id { get; }

        bool Ok { get; }

        IInspector Init(string id, string path);
    }
}
