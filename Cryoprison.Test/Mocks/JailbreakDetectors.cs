using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cryoprison.Test.Mocks
{
    public class JailbreakDetector : Cryoprison.JailbreakDetector
    {
        public JailbreakDetector(IEnumerable<IInspector> inspectors = null)
        {
            base.AddInspectors(inspectors);
        }

        public List<IInspector> GetInspectors()
        {
            return this.inspectors.ToList();
        }

        public bool HasRun
        {
            get
            {
                return this.jailbreaks != null;
            }
        }
    }

}
