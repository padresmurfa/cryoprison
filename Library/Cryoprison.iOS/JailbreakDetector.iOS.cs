using System;
using System.Collections;
using System.Collections.Generic;
using Cryoprison.Inspectors;

namespace Cryoprison.iOS
{
    /// <summary>
    /// iOS implementation of the Jailbreak Detector.
    /// </summary>
    public class JailbreakDetector : Cryoprison.JailbreakDetector
    {
        /// <summary>
        /// These directories should not be present in the first place.
        /// </summary>
        private Checks DirectoriesShouldNotBePresent
        {
            get
            {
                return new Checks(this.Env).
                  Add("SBSETTINGS", "/private/var/mobileLibrary/SBSettingsThemes").
                  Add("STASH", "/private/var/stash").
                  Add("APT", "/private/var/lib/apt", "/private/var/cache/apt", "/var/lib/apt", "/private/var/lib/apt", "/private/etc/apt", "/etc/apt", "/private/cache/apt").
                  Add("CYDIA", "/private/var/lib/cydia", "/var/lib/cydia", "/usr/libexec/cydia");
            }
        }

        /// <summary>
        /// These files should never be accessible.  That is, we shouldn't be
        /// able to read them.
        /// </summary>
        private Checks FilesShouldNotBeAccessible
        {
            get
            {
                return new Checks(this.Env).
                  Add("CYDIA", "/Applications/Cydia.app").
                  Add("APT", "/etc/apt");
            }
        }

        /// <summary>
        /// The following files are used for development, and should only be
        /// readable on simulators.  If we are running in 'simulator friendly'
        /// mode, then we won't check that they exist.  Very similar to the
        /// not-present check, but tests whether we can open and read them.
        /// </summary>
        private Checks DeveloperFilesShouldNotBeAccessible
        {
            get
            {
                return new Checks(this.Env).
                  Add("BASH", "/bin/bash").
                  Add("SH", "/bin/sh").
                  Add("SSHD", "/usr/bin/sshd", "/usr/sbin/sshd").
                  Add("SSH", "/usr/bin/ssh");
            }
        }

        /// <summary>
        /// The following locations should be read-only, and thus we shouldn't
        /// be able to create / write / delete these files.  Note, we will
        /// try to delete these files, so don't add anything here that is not
        /// supposed to be destroyed.
        /// </summary>
        private Checks FilesShouldNotBeDestructivelyWritable
        {
            get
            {
                return new Checks(this.Env).
                    Add("PRIVATE", "/private/cryoprison");
            }
        }

        /// <summary>
        /// The following files are found on jailbroken devices.  Mostly these
        /// are related to specific apps / package managers
        /// </summary>
        private Checks FilesShouldNotBePresent
        {
            get
            {
                return new Checks(this.Env).
                  Add("BLACKRA1N", "/Applications/blackra1n.app").
                  Add("CYDIA", "/Applications/Cydia.app", "/private/var/tmp/cydia.log", "/System/Library/LaunchDaemons/com.saurik.Cydia.Startup.plist").
                  Add("FAKECARRIER", "/Applications/FakeCarrier.app").
                  Add("INTELLISCREEN", "/Applications/IntelliScreen.app").
                  Add("MXTUBE", "/Applications/MxTube.app").
                  Add("ROCKAPP", "/Applications/RockApp.app").
                  Add("SBSETTINGS", "/Applications/SBSettings.app").
                  Add("SNOOPITCONFIG", "/Applications/Snoop-it Config.app").
                  Add("WINTERBOARD", "/Applications/WinterBoard.app").
                  Add("IKEYBBOT", "/System/Library/LaunchDaemons/com.ikey.bbot.plist").
                  Add("CYDIA_SUBSTRATE", "/Library/MobileSubstrate/MobileSubstrate.dylib", "/Library/MobileSubstrate/DynamicLibraries/xCon.dylib").
                  Add("SYSLOG", "/var/log/syslog", "/private/var/log/syslog").
                  Add("DEBIANPKG", "/private/etc/dpkg/origins/debian").
                  Add("ICY", "/Applications/Icy.app");
            }
        }

        /// <summary>
        /// The following files are used for development, and should only be
        /// present on simulators.  If we are running in 'simulator friendly'
        /// mode, then we won't check that they exist.  Very similar to the
        /// not-accessible check, but tests whether we can find them in a
        /// directory listing.
        /// </summary>
        private Checks DeveloperFilesShouldNotBePresent
        {
            get
            {
                return new Checks(this.Env).
                  Add("SFTPSERVER", "/usr/libexec/sftp-server").
                  Add("BASH", "/bin/bash").
                  Add("SH", "/bin/sh").
                  Add("SSH", "/usr/bin/ssh").
                  Add("SSHD", "/usr/bin/sshd", "/usr/sbin/sshd", "/private/etc/ssh/sshd_config", "/etc/ssh/sshd_config", "/usr/libexec/ssh-keysign");
            }
        }

        /// <summary>
        /// The following urls should not be openable by the app.  Cydia is a
        /// jailbreak package manager which exposes the cydia uri scheme.  It
        /// should obviously not be installed.
        /// 
        /// REMEMBER: add cydia to info.plist to be able to detect its presence,
        /// otherwise iOS will falsely report that the url can't be opened after
        /// a moderate amount of checks have been made:
        ///            
        ///    <key>LSApplicationQueriesSchemes</key>
        ///    <array>
        ///        <string>cydia</string>
        ///    </array>
        /// </summary>
        ///
        private Checks UrlsShouldNotBeOpenable
        {
            get
            {
                return new Checks(this.Env).
                  Add("CYDIA", "cydia://package/com.example.package");
            }
        }

        /// <summary>
        /// The following paths are sometimes turned into symbolic links on
        /// jailbroken devices.
        /// </summary>
        private Checks PathsShouldNotBeSymbolicLinks
        {
            get
            {
                return new Checks(this.Env).
                     Add("APPLICATIONS", "/Applications").
                     Add("RINGTONES", "/var/stash/Library/Ringtones").
                     Add("WALLPAPER", "/var/stash/Library/Wallpaper").
                     Add("INCLUDE", "/var/stash/usr/include").
                     Add("LIBEXEC", "/var/stash/usr/libexec").
                     Add("SHARE", "/var/stash/usr/share").
                     Add("DARWIN9", "/var/stash/usr/arm-apple-darwin9");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Cryoprison.iOS.JailbreakDetector"/> class.
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

            // TODO: There is some interesting obfuscated logic here
            //   that would be worth investigating further.  Might be some
            //   cool tricks there.
            //   https://github.com/masbog/isJB/blob/master/JailbreakDetection/JBDetect.m
            // 
            // later:  add forking, info plist and exe modification detection
            // 
            // maybe: obfuscate self to hide from sneaky jailbreaks.

            if (!simulatorFriendly.Value)
            {
                this.AddInspectors(new[] { new ShouldBeMobileProvisioned() });
            }

            this.AddInspectors(PathsShouldNotBeSymbolicLinks.GetInspectors<Cryoprison.iOS.PlatformSpecific.PathNotSymbolicLink>());

            this.AddInspectors(FilesShouldNotBePresent.GetInspectors<FileNotPresent>());

            this.AddInspectors(FilesShouldNotBeAccessible.GetInspectors<FileNotAccessible>());

            if (!simulatorFriendly.Value)
            {
                this.AddInspectors(DeveloperFilesShouldNotBePresent.GetInspectors<FileNotPresent>());

                this.AddInspectors(DeveloperFilesShouldNotBeAccessible.GetInspectors<FileNotAccessible>());
            }

            this.AddInspectors(DirectoriesShouldNotBePresent.GetInspectors<DirectoryNotPresent>());

            this.AddInspectors(FilesShouldNotBeDestructivelyWritable.GetInspectors<FileNotDestructivelyWritable>());

            this.AddInspectors(UrlsShouldNotBeOpenable.GetInspectors<Cryoprison.iOS.PlatformSpecific.UrlNotOpenable>());

        }
    }
}
