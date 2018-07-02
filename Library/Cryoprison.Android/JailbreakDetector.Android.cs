using System;
using System.Collections;
using System.Collections.Generic;
using Cryoprison.Inspectors;
using Cryoprison.Android.PlatformSpecific;

namespace Cryoprison.Android
{
    /// <summary>
    /// Android implementation of the Jailbreak Detector.
    /// </summary>
    public class JailbreakDetector : Cryoprison.JailbreakDetector
    {
        /// <summary>
        /// The SU executable should not be found, as its used to gain root access
        /// </summary>
        private static readonly Checks ExecutablesShouldNotBeFound = new Checks().
            Add("SU", "su");

        /// <summary>
        /// The kernel should not be built by a third-party developer.
        /// </summary>
        private static readonly Checks BuildTagsShouldNotBeFound = new Checks().
            Add("TESTKERNEL", "test-keys");

        /// <summary>
        /// Known root apps should not be installed
        /// </summary>
        private static readonly Checks KnownRootApps = new Checks().
            Add("NOSHUFOU", "com.noshufou.android.su", "com.noshufou.android.su.elite").
            Add("CHAINFIRE", "eu.chainfire.supersu").
            Add("KOUSHIKDUTTA_SUPERUSER", "com.koushikdutta.superuser").
            Add("THIRDPARTY", "com.thirdparty.superuser").
            Add("YELLOWES", "com.yellowes.su").
            Add("TOPJOHNWU", "com.topjohnwu.magisk");

        /// <summary>
        /// Dangerous apps should not be installed
        /// </summary>
        private static readonly Checks KnownDangerousApps = new Checks().
            Add("KOUSHIKDUTTA_ROMMANAGER", "com.koushikdutta.rommanager", "com.koushikdutta.rommanager.license").
            Add("DIMONVIDEO", "com.dimonvideo.luckypatcher").
            Add("CHELPUS", "com.chelpus.lackypatch", "com.chelpus.luckypatcher").
            Add("RAMDROID", "com.ramdroid.appquarantine", "com.ramdroid.appquarantinepro").
            Add("IABSCOIN", "com.android.vending.billing.InAppBillingService.COIN");

        /// <summary>
        /// Root cloking apps should not be installed
        /// </summary>
        private static readonly Checks KnownRootCloackingPackages = new Checks().
            Add("DEVADVANCE", "com.devadvance.rootcloak", "com.devadvance.rootcloakplus").
            Add("XPOSED", "de.robv.android.xposed.installer").
            Add("SAURIK", "com.saurik.substrate").
            Add("ZACHSPONG", "com.zachspong.temprootremovejb").
            Add("AMPHORAS", "com.amphoras.hidemyroot", "com.amphoras.hidemyrootadfree").
            Add("FORMYHM", "com.formyhm.hiderootPremium", "com.formyhm.hideroot")
            ;

        /// <summary>
        /// The root directories where we are likely to find executable files
        /// </summary>
        private static readonly string[] ExecutableRoots = {
            "/data/local/",
            "/data/local/bin/",
            "/data/local/xbin/",
            "/sbin/",
            "/su/bin/",
            "/system/bin/",
            "/system/bin/.ext/",
            "/system/bin/failsafe/",
            "/system/sd/xbin/",
            "/system/usr/we-need-root/",
            "/system/xbin/",
            "/cache/",
            "/data/",
            "/dev/"
        };

        /// <summary>
        /// Some dangerous executables that should not be installed
        /// </summary>
        private static readonly Checks DangerousExecutables = new Checks().
            AddRoots(ExecutableRoots).
            Add("SU", "su").
            Add("MAGISK", "magisk");

        /// <summary>
        /// The following directories tend to be made writable be jailbreak
        /// tools.  So we try to create / update / delete a file in them.
        /// </summary>
        private static readonly Checks PathsThatShouldNotBeWritable = new Checks().
            Add("SYSTEM", "/system/cryoprison").
            Add("SYSTEM_BIN", "/system/bin/cryoprison").
            Add("SYSTEM_SBIN", "/system/sbin/cryoprison").
            Add("SYSTEM_XBIN", "/system/xbin/cryoprison").
            Add("VENDOR_BIN", "/vendor/bin/cryoprison").
            Add("SBIN", "/sbin/cryoprison").
            Add("ETC", "/etc/cryoprison");

        /// <summary>
        /// The following properties should not be set
        /// </summary>
        private static readonly Checks PropsThatShouldNotBeSet = new Checks().
            Add("RO_INSECURE", "ro.secure=0");

        /// <summary>
        /// The following properties should not be set unless we are debugging,
        /// that is in a simulator friendly mode.
        /// </summary>
        private static readonly Checks PropsThatShouldNotBeSetUnlessDebugging = new Checks().
            Add("RO_DEBUGGABLE", "ro.debuggable=1");
        
        /// <summary>
        /// Gets or sets the global jailbreak report hook, which is intended
        /// for debugging and logging.
        /// </summary>
        public static Action<string> OnJailbreakReported
        {
            get { return Cryoprison.Reporter.OnJailbreakReported; }
            set { Cryoprison.Reporter.OnJailbreakReported = value; }
        }

        /// <summary>
        /// Gets or sets the global exception report hook, which is intended
        /// for debugging and logging faults during jailbreak detection.
        /// </summary>
        /// <value>The on exception reported.</value>
        public static Action<string, Exception> OnExceptionReported
        {
            get { return Cryoprison.Reporter.OnExceptionReported; }
            set { Cryoprison.Reporter.OnExceptionReported = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.Android.JailbreakDetector"/> class.
        /// By default, the jailbreak detector will be 'simulator friendly'
        /// when built in debug mode, and hostile towards the simulator in release
        /// builds.  This is intended to aid debugging, since the simulator will
        /// appear to be jailbroken due to installed debugging tools.
        /// </summary>
        /// <param name="simulatorFriendly">Simulator friendly.</param>
        public JailbreakDetector(bool? simulatorFriendly = null)
        {
            if (simulatorFriendly == null)
            {
#if DEBUG
                simulatorFriendly = true;
#else
                simulatorFriendly = false;
#endif
            }

            if (!simulatorFriendly.Value)
            {
                this.AddInspectors(PropsThatShouldNotBeSetUnlessDebugging.GetInspectors<ShouldNotHavePropValues>());
            }

            this.AddInspectors(ExecutablesShouldNotBeFound.GetInspectors<ShouldNotBeAbleToLocateFile>());
            this.AddInspectors(BuildTagsShouldNotBeFound.GetInspectors<ShouldNotHaveSpecificBuildTags>());
            this.AddInspectors(KnownRootApps.GetInspectors<ShouldNotHavePackageInstalled>());
            this.AddInspectors(KnownDangerousApps.GetInspectors<ShouldNotHavePackageInstalled>());
            this.AddInspectors(KnownRootCloackingPackages.GetInspectors<ShouldNotHavePackageInstalled>());
            this.AddInspectors(DangerousExecutables.GetInspectors<FileNotPresent>());
            this.AddInspectors(PathsThatShouldNotBeWritable.GetInspectors<FileNotDestructivelyWritable>());
            this.AddInspectors(PropsThatShouldNotBeSet.GetInspectors<ShouldNotHavePropValues>());
        }
    }
}
