using System;
using System.Collections.Generic;

namespace Cryoprison.Inspectors
{
    public class UrlNotOpenable : IInspector
    {
        private string id;
        private string url;

        public IInspector Init(string id, string url)
        {
            this.id = string.Format("URL_{0}_SHOULD_NOT_BE_OPENABLE", id);
            this.url = url;

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
                return !IsUrlOpenable(url);
            }
        }

        protected virtual bool CanOpenUrl(string url)
        {
            return true;
        }

        public bool IsUrlOpenable(string url)
        {
            try
            {
                return CanOpenUrl(url);
            }
            catch (Exception ex)
            {
                Reporter.ReportException($"IsUrlOpenable bombed for {url}", ex);
                return false;
            }
        }
    }
}
