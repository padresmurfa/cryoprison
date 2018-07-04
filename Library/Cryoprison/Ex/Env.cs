using S=System;
using Cryoprison.Ex;

namespace Cryoprison.Ex
{
    public class Env
    {
        public System System { get; private set; } = new System();

        public Reporter Reporter { get; set; } = new Reporter();
    }
}
