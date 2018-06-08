using System;
using System.Collections.Generic;

namespace Cryoprison
{
    public class Check
    {
        public string Id { get; set; }

        public string Value { get; set; }
    }

    public class Checks
    {
        private List<Check> checks { get; set; } = new List<Check>();

        public Checks Add(string id, string val)
        {
            this.checks.Add(new Check { Id = id, Value = val });

            return this;
        }

        public Checks Add(string id, params string[] vals)
        {
            foreach (var val in vals)
            {
                this.Add(id, val);
            }

            return this;
        }
        
        public IEnumerable<IInspector> GetInspectors<T>()
            where T : IInspector, new()
        {
            var retval = new List<IInspector>();

            foreach (var check in checks)
            {
                retval.Add(new T().Init(check.Id.ToUpperInvariant(), check.Value));
            }

            return retval;
        }
    }
}
