using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Java.IO
{
    public class InputStream
	{
        private List<string> mockInput = new List<string>();

        public IEnumerable<string> MockInput
        {
            get
            {
                return mockInput;
            }

            set
            {
                mockInput = value?.ToList();
            }
        }
	}
}
