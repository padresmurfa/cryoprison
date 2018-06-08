using System;
using Cryoprison;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cryoprison.Inspectors;

namespace Cryoprison
{
    public class JailbreakDetector : IJailbreakDetector
    {
        private List<string> jailbreaks = null;

        private List<IInspector> inspectors;

        public JailbreakDetector(IEnumerable<IInspector> inspectors = null)
        {
            var type = typeof(IInspector);

            this.inspectors = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .Select(x => Instantiate(x)).Where(x => x != null)
                .ToList();

            if (inspectors != null)
            {
                this.inspectors.AddRange(inspectors);
            }
        }

        protected void AddInspectors(IEnumerable<IInspector> inspectors)
        {
            this.inspectors.AddRange(inspectors);
        }

        public void Refresh()
        {
            lock (this)
            {
                this.jailbreaks = null;
            }
        }

        public IEnumerable<string> Violations
        {
            get
            {
                lock (this)
                {
                    if (this.jailbreaks == null)
                    {
                        var jailbreaks = this.inspectors.Select(Inspect).Where(x => x != null);

                        this.jailbreaks = jailbreaks.ToList();
                    }
                    return this.jailbreaks;
                }
            }
        }

        public bool IsJailbroken
        {
            get
            {
                return this.Violations.Any();
            }
        }

        private static Type[] nopes = new [] { typeof(UrlNotOpenable), typeof(FileNotPresent), typeof(DirectoryNotPresent), typeof(FileNotAccessible), typeof(FileNotDestructivelyWritable),  typeof(IInspector) };

        public static IInspector Instantiate(Type type)
        {
            if (nopes.Any(x => x == type))
            {
                return null;
            }

            try
            {
                return (IInspector)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                Reporter.ReportException("IInspector.Instantiate failed", ex);

                return null;
            }
        }

        private static string Inspect(IInspector inspector)
        {
            var id = inspector.Id;

            try
            {
                if (inspector.Ok)
                {
                    return null;   
                }

                Reporter.ReportJailbreak(id);

                return id;
            }
            catch (Exception ex)
            {
                Reporter.ReportException("Inspector crashed: " + id, ex);

                return null;
            }
        }
    }
}
