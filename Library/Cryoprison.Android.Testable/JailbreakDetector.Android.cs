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
        private Checks ExecutablesShouldNotBeFound
        {
            get
            {
                return new Checks(this.Env).
                    Add("SU", "su");
            }
        }

        /// <summary>
        /// The kernel should not be built by a third-party developer.
        /// </summary>
        private Checks BuildTagsShouldNotBeFound
        {
            get
            {
                return new Checks(this.Env).
                    Add("TESTKERNEL", "test-keys");
            }
        }

        /// <summary>
        /// Apps that provide root access should not be installed
        /// </summary>
        private Checks RootAppsShouldNotBeInstalled
        {
            get
            {
                return new Checks(this.Env).
                    Add("CHAINFIRE", "eu.chainfire.supersu").
                    Add("KOUSHIKDUTTA_SUPERUSER", "com.koushikdutta.superuser").
                    Add("NOSHUFOU", "com.noshufou.android.su", "com.noshufou.android.su.elite").
                    Add("THIRDPARTY", "com.thirdparty.superuser").
                    Add("TOPJOHNWU", "com.topjohnwu.magisk").
                    Add("YELLOWES", "com.yellowes.su");
            }
        }

        /// <summary>
        /// Dangerous apps should not be installed
        /// </summary>
        private Checks DangerousAppsShouldNotBeInstalled
        {
            get
            {
                return new Checks(this.Env).
                    Add("CHELPUS", "com.chelpus.lackypatch", "com.chelpus.luckypatcher").
                    Add("DIMONVIDEO", "com.dimonvideo.luckypatcher").
                    Add("IABSCOIN", "com.android.vending.billing.InAppBillingService.COIN").
                    Add("KOUSHIKDUTTA_ROMMANAGER", "com.koushikdutta.rommanager", "com.koushikdutta.rommanager.license").
                    Add("RAMDROID", "com.ramdroid.appquarantine", "com.ramdroid.appquarantinepro");
            }
        }

        /// <summary>
        /// Root cloaking apps should not be installed
        /// </summary>
        private Checks RootCloakingAppsShouldNotBeInstalled
        {
            get
            {
                return new Checks(this.Env).
                    Add("AMPHORAS", "com.amphoras.hidemyroot", "com.amphoras.hidemyrootadfree").
                    Add("DEVADVANCE", "com.devadvance.rootcloak", "com.devadvance.rootcloakplus").
                    Add("FORMYHM", "com.formyhm.hiderootPremium", "com.formyhm.hideroot").
                    Add("SAURIK", "com.saurik.substrate").
                    Add("XPOSED", "de.robv.android.xposed.installer").
                    Add("ZACHSPONG", "com.zachspong.temprootremovejb");
            }
        }

        /// <summary>
        /// The root directories where we are likely to find executable files
        /// </summary>
        public static string[] CommonExecutableRootDirectories = {
            "/cache/",
            "/data/",
            "/data/local/",
            "/data/local/bin/",
            "/data/local/xbin/",
            "/dev/",
            "/sbin/",
            "/su/bin/",
            "/system/bin/",
            "/system/bin/.ext/",
            "/system/bin/failsafe/",
            "/system/sd/xbin/",
            "/system/usr/we-need-root/",
            "/system/xbin/"
        };

        /// <summary>
        /// Some dangerous executables that should not be installed
        /// </summary>
        private Checks DangerousExecutablesShouldNotBeInstalled
        {
            get
            {
                return new Checks(this.Env).
                    AddRoots(CommonExecutableRootDirectories).
                        Add("SU", "su").
                        Add("MAGISK", "magisk");
            }
        }

        /// <summary>
        /// The following directories tend to be made writable be jailbreak
        /// tools.  So we try to create / update / delete a file in them.
        /// </summary>
        private Checks PathsShouldNotBeWritable
        {
            get
            {
                return new Checks(this.Env).
                    Add("ETC", "/etc/cryoprison").
                    Add("SBIN", "/sbin/cryoprison").
                    Add("SYSTEM", "/system/cryoprison").
                    Add("SYSTEM_BIN", "/system/bin/cryoprison").
                    Add("SYSTEM_SBIN", "/system/sbin/cryoprison").
                    Add("SYSTEM_XBIN", "/system/xbin/cryoprison").
                    Add("VENDOR_BIN", "/vendor/bin/cryoprison");
            }
        }

        /// <summary>
        /// The following properties should not be set
        /// </summary>
        private Checks PropsShouldNotBeSet
        {
            get
            {
                return new Checks(this.Env).
                    Add("RO_INSECURE", "ro.secure=0");
            }
        }

        /// <summary>
        /// The following properties should not be set unless we are debugging,
        /// that is in a simulator friendly mode.
        /// </summary>
        private Checks PropsShouldNotBeSetUnlessDebugging
        {
            get
            {
                return new Checks(this.Env).
                    Add("RO_DEBUGGABLE", "ro.debuggable=1");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.Android.JailbreakDetector"/> class.
        /// By default, the jailbreak detector will be 'simulator friendly'
        /// when built in debug mode, and hostile towards the simulator in release
        /// builds.  This is intended to aid debugging, since the simulator will
        /// appear to be jailbroken due to installed debugging tools.
        /// </summary>
        /// <param name="simulatorFriendly">Simulator friendly.</param>
        public JailbreakDetector(Cryoprison.Ex.Env env, bool? simulatorFriendly = null)
            : base(env)
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
                this.AddInspectors(PropsShouldNotBeSetUnlessDebugging.GetInspectors<ShouldNotHavePropValues>());
            }

            this.AddInspectors(ExecutablesShouldNotBeFound.GetInspectors<ShouldNotBeAbleToLocateFile>());
            this.AddInspectors(BuildTagsShouldNotBeFound.GetInspectors<ShouldNotHaveSpecificBuildTags>());
            this.AddInspectors(RootAppsShouldNotBeInstalled.GetInspectors<ShouldNotHavePackageInstalled>());
            this.AddInspectors(DangerousAppsShouldNotBeInstalled.GetInspectors<ShouldNotHavePackageInstalled>());
            this.AddInspectors(RootCloakingAppsShouldNotBeInstalled.GetInspectors<ShouldNotHavePackageInstalled>());
            this.AddInspectors(DangerousExecutablesShouldNotBeInstalled.GetInspectors<FileNotPresent>());
            this.AddInspectors(PathsShouldNotBeWritable.GetInspectors<FileNotDestructivelyWritable>());
            this.AddInspectors(PropsShouldNotBeSet.GetInspectors<ShouldNotHavePropValues>());
        }
    }
}
