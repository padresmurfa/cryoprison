using System;
namespace Cryoprison.Test.Mocks
{
    public class PathNotSymbolicLink : Cryoprison.Inspectors.PathNotSymbolicLink
    {
        public bool SymbolicLink { get; set; }
        public Exception ThrowException { get; set; }

        protected override bool IsSymLink(string path)
        {
            if (ThrowException != null)
            {
                throw new Exception("BOOM!", ThrowException);
            }
            return SymbolicLink;
        }
    }
}
