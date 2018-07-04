using System;
using System.Collections.Generic;
using Java.IO;

namespace Cryoprison.Android.PlatformSpecific
{
    /// <summary>
    /// Determines if we can locate a file using the which command
    /// </summary>
    public class ShouldNotBeAbleToLocateFile : IInspector
    {
        private Ex.Env Env;
        private string id;
        private string path;

        /// <inheritdoc/>
        public IInspector Init(Ex.Env env, string id, string path)
        {
            this.Env = env;
            this.id = id;
            this.path = path;
            return this;
        }

        /// <inheritdoc/>
        public string Id
        {
            get
            {
                var idUpper = this.id.ToUpperInvariant();
                return $"SHOULD_NOT_LOCATE_{idUpper}";
            }
        }

        /// <inheritdoc/>
        public bool Ok
        {
            get
            {
                return !CanLocateFile(this.path);
            }
        }

        /// <summary>
        /// Determines if the app has been rooted, by checking if we can
        /// locate the specified file.
        /// </summary>
        public bool CanLocateFile(string target)
        {
            try
            {
                using (var process = Java.Lang.Runtime.GetRuntime().Exec(new[] { "which", target }))
                {
                    try
                    {
                        using (var inputStream = new InputStreamReader(process.InputStream))
                        {
                            using (var inputReader = new BufferedReader(inputStream))
                            {
                                // will return the path of the target, if found,
                                // otherwise an empty response.
                                return !string.IsNullOrWhiteSpace(inputReader.ReadLine());
                            }
                        }
                    }
                    finally
                    {
                        process.Destroy();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Env.Reporter.ReportException($"CanLocateFile bombed", ex);
                return false;
            }
        }
    }
}
