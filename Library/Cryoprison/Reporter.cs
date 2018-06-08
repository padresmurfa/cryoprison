using System;

namespace Cryoprison
{
    public static class Reporter
    {
        public static Action<string> OnJailbreakReported { get; set; }

        public static Action<string, Exception> OnExceptionReported { get; set; }

        public static void ReportException(string message, Exception exception)
        {
            try
            {
                var r = OnExceptionReported;
                if (r != null)
                {
                    r(message, exception);
                }
            }
            catch (Exception)
            {
                // We don't forward exceptions from the reporting mechanism.
            }
        }

        public static void ReportJailbreak(string id)
        {
            try
            {
                var r = OnJailbreakReported;
                if (r != null)
                {
                    r(id);
                }
            }
            catch (Exception)
            {
                // We don't forward exceptions from the reporting mechanism.
            }
        }
    }
}
