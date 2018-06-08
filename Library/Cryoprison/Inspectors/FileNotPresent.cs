using System;
using System.Collections.Generic;

namespace Cryoprison.Inspectors
{
    public class FileNotPresent : IInspector
    {
        private string id;
        private string path;

        public IInspector Init(string id, string path)
        {
            this.id = string.Format("FILE_{0}_SHOULD_NOT_BE_PRESENT", id);
            this.path = path;

            return this;
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public bool Ok
        {
            get
            {
                return !IsFilePresent(path);
            }
        }

        public static bool IsFilePresent(string path)
        {
            try
            {
                return System.IO.File.Exists(path);
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"IsFilePresent bombed for {path}", ex);
                return false;
            }
        }
    }
}
