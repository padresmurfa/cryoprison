using System;

namespace Cryoprison.Test.Mocks
{
    public class Inspector : IInspector
    {
        protected bool bombDuringInit;
        protected bool bombDuringRun;

        public Ex.Env env;
        public string checkId;
        public string val;

        public bool Ran
        {
            get; private set;
        }

        public string Value
        {
            get
            {
                return this.val;
            }
        }

        public string Id
        {
            get
            {
                return $"MOCK_{this.checkId}_ID";
            }
        }

        public bool Ok
        {
            get
            {
                if (this.bombDuringRun)
                {
                    throw new Exception("bomb during run");
                }

                this.Ran = true;
                return this.checkId == null || this.checkId.StartsWith("OK");
            }
        }

        public IInspector Init(Ex.Env env, string checkId, string val)
        {
            if (this.bombDuringInit && checkId == "FUBAR")
            {
                throw new Exception("bomb during init");
            }

            this.env = env;
            this.checkId = checkId;
            this.val = val;
            return this;
        }
    }

    public class InspectorThatBombsDuringConstruction : Inspector
    {
        public InspectorThatBombsDuringConstruction()
        {
            throw new Exception("bomb during construction");
        }

    }

    public class InspectorThatBombsDuringInit : Inspector
    {
        public InspectorThatBombsDuringInit()
        {
            this.bombDuringInit = true;
        }
    }

    public class InspectorThatBombsDuringRun : Inspector
    {
        public InspectorThatBombsDuringRun()
        {
            this.bombDuringRun = true;
        }
    }
}
