using System;
using System.Collections.Generic;

namespace Cryoprison.Inspectors
{
    public class FileNotAccessible : IInspector
    {
        private string id;
        private string path;

        public IInspector Init(string id, string path)
        {
            this.id = string.Format("FILE_{0}_SHOULD_NOT_BE_ACCESSIBLE", id);
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
                return !IsFileAccessible(path);
            }
        }

        public static bool IsFileAccessible(string path)
        {
            try
            {
                using (var file = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite | System.IO.FileShare.Delete))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"IsFileAccessible bombed for {path}", ex);
                return false;
            }
        }
    }
}
