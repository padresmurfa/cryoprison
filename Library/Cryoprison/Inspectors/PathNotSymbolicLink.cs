using System;
using System.Collections.Generic;

namespace Cryoprison.Inspectors
{
    public class PathNotSymbolicLink : IInspector
    {
        private string id;
        private string path;

        public IInspector Init(string id, string path)
        {
            this.id = string.Format("PATH_{0}_SHOULD_NOT_REFER_TO_SYMBOLIC_LINK", id);
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
                return !DoesPathPointToSymbolicLink(path);
            }
        }

        protected virtual bool IsSymLink(string path)
        {
            return false;
        }

        public bool DoesPathPointToSymbolicLink(string path)
        {
            try
            {
                return IsSymLink(path);
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"DoesPathPointToSymbolicLink bombed for {path}", ex);
                return false;
            }
        }
    }
}
