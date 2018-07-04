using S=System;

namespace Cryoprison.Ex
{
	public class SystemIODirectory
	{
        private S.Func<string, bool> exists;

        public S.Func<string, bool> Exists
        {
            get
            {
                return this.exists ?? S.IO.Directory.Exists;
            }

            set
            {
                this.exists = value;
            }
        }
	}
}
