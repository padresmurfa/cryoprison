using System;
namespace Cryoprison.Test.Mocks
{
    public class UrlNotOpenable : Cryoprison.Inspectors.UrlNotOpenable
    {
        public bool CanOpen { get; set; }
        public Exception ThrowException { get; set; }

        protected override bool CanOpenUrl(string path)
        {
            if (ThrowException != null)
            {
                throw new Exception("BOOM!", ThrowException);
            }
            return CanOpen;
        }
    }
}
