using S=System;

namespace Cryoprison.Ex
{
    public class SystemIOFile
    {
        private S.Func<string, S.IO.FileMode, S.IO.FileAccess, S.IO.FileShare, S.IO.Stream> open;
        private S.Func<string, bool> exists;
        private S.Action<string> delete;

        public S.Func<string, S.IO.FileMode, S.IO.FileAccess, S.IO.FileShare, S.IO.Stream> Open
        {
            get
            {
                return this.open ?? S.IO.File.Open;
            }

            set
            {
                this.open = value;
            }
        }

        public S.Func<string, bool> Exists
        {
            get
            {
                return this.exists ?? S.IO.File.Exists;
            }

            set
            {
                this.exists = value;
            }
        }

        public S.Action<string> Delete
        {
            get
            {
                return this.delete ?? S.IO.File.Delete;
            }

            set
            {
                this.delete = value;
            }
        }    }
}
