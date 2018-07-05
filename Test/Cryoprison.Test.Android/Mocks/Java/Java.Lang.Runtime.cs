using System;

namespace Java.Lang
{
    public class Runtime
    {
        public static Runtime MockRuntime { get; set; } = new Runtime();

        public Func<string[], Process> MockExec { get; set; } = (p) => { return new Process(); };

        public static Runtime GetRuntime()
        {
            return MockRuntime;
        }

        public Process Exec(params string[] args)
        {
            return MockExec(args);
        }
    }
}
