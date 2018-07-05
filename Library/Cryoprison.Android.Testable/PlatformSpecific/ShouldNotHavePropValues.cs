using System;
using System.Collections.Generic;

namespace Cryoprison.Android.PlatformSpecific
{
    /// <summary>
    /// Determines if OS properties have specific values
    /// </summary>
    public class ShouldNotHavePropValues : IInspector
    {
        private Ex.Env Env;
        private string id;

        private string key;
        private string value;

        /// <inheritdoc/>
        public IInspector Init(Ex.Env env, string id, string path)
        {
            try
            {
                this.Env = env;
                this.id = id;

                if (string.IsNullOrEmpty(path) || !path.Contains("="))
                {
                    throw new ArgumentException(path);
                }

                this.key = path.Split('=')[0];
                this.value = path.Substring(1 + this.key.Length);
            }
            catch (Exception ex)
            {
                this.key = null;
                this.value = null;
                this.Env.Reporter.ReportException($"ShouldNotHavePropValues init bombed for {id}={path}", ex);
            }
            return this;
        }

        /// <inheritdoc/>
        public string Id
        {
            get
            {
                var idUpper = this.id.ToUpperInvariant();
                return $"SHOULD_NOT_HAVE_PROP_VALUES_{idUpper}";
            }
        }

        /// <inheritdoc/>
        public bool Ok
        {
            get
            {
                return !HasPropValue(this.key, this.value);
            }
        }

        /// <summary>
        /// Determines if the app has been rooted, by checking if the specified
        /// OS property has the specified value
        /// </summary>
        public bool HasPropValue(string key, string value)
        {
            try
            {
                using (var process = Java.Lang.Runtime.GetRuntime().Exec("getprop"))
                {
                    try
                    {
                        if (process.InputStream == null)
                        {
                            return false;
                        }

                        using (var inputStreamReader = new Java.IO.InputStreamReader(process.InputStream))
                        {
                            using (var inputReader = new Java.IO.BufferedReader(inputStreamReader))
                            {
                                for (var i = 0; i < 1000; ++i)
                                {
                                    var line = inputReader.ReadLine();
                                    if (line == null)
                                    {
                                        break;
                                    }

                                    var k = $"[{key}]";
                                    var v = $"[{value}]";

                                    if (line.Contains(k) && line.Contains(v))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        process.Destroy();
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                this.Env.Reporter.ReportException($"HasPropValue bombed for {this.id} ({key}={value})", ex);
                return false;
            }
        }
    }
}
