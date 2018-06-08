using System;
using System.Collections.Generic;

namespace Cryoprison.Inspectors
{
    public class DirectoryNotPresent : IInspector
    {
        private string id;
        private string path;

        public IInspector Init(string id, string path)
        {
            this.id = string.Format("DIRECTORY_{0}_SHOULD_NOT_BE_PRESENT",id);
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
                return !IsDirectoryPresent(path);
            }
        }

        public static bool IsDirectoryPresent(string path)
        {
            try
            {
                return System.IO.Directory.Exists(path);
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"IsDirectoryPresent bombed for {path}", ex);
                return false;
            }
        }
    }
}
