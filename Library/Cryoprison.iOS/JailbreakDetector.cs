using System;
using System.Collections;
using System.Collections.Generic;
using Cryoprison.Inspectors;

namespace Cryoprison.iOS
{
    public class JailbreakDetector : Cryoprison.JailbreakDetector
    {
        private static readonly Checks DirectoriesShouldNotBePresent = new Checks().
          Add("SBSETTINGS", "/private/var/mobileLibrary/SBSettingsThemes").
          Add("STASH", "/private/var/stash").
          Add("APT", "/private/var/lib/apt", "/private/var/cache/apt", "/var/lib/apt", "/private/var/lib/apt", "/private/etc/apt", "/etc/apt", "/private/cache/apt").
          Add("CYDIA", "/private/var/lib/cydia", "/var/lib/cydia", "/usr/libexec/cydia");
        
        private static readonly Checks FilesShouldNotBeAccessible = new Checks().
          Add("CYDIA", "/Applications/Cydia.app").
          Add("APT", "/etc/apt");
        
        private static readonly Checks DeveloperFilesShouldNotBeAccessible = new Checks().
          Add("BASH", "/bin/bash").
          Add("SH", "/bin/sh").
          Add("SSHD", "/usr/bin/sshd", "/usr/sbin/sshd").
          Add("SSH", "/usr/bin/ssh");
        
        private static readonly Checks FilesShouldNotBeDestructivelyWritable = new Checks().
          Add("PRIVATE", "/private/cryoprison");
        
        private static readonly Checks FilesShouldNotBePresent = new Checks().
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
        
        private static readonly Checks DeveloperFilesShouldNotBePresent = new Checks().
          Add("SFTPSERVER", "/usr/libexec/sftp-server").
          Add("BASH", "/bin/bash").
          Add("SH", "/bin/sh").
          Add("SSH", "/usr/bin/ssh").
          Add("SSHD", "/usr/bin/sshd", "/usr/sbin/sshd", "/private/etc/ssh/sshd_config", "/etc/ssh/sshd_config", "/usr/libexec/ssh-keysign");

        /*
            REMEMBER: add cydia to info.plist to be able to detect it:
            
            <key>LSApplicationQueriesSchemes</key>
            <array>
                <string>cydia</string>
            </array>
        */
        private static readonly Checks UrlsShouldNotBeOpenable = new Checks().
          Add("CYDIA", "cydia://package/com.example.package");

        private static readonly Checks PathsShouldNotBeSymbolicLinks = new Checks().
             Add("APPLICATIONS", "/Applications").
             Add("RINGTONES", "/var/stash/Library/Ringtones").
             Add("WALLPAPER", "/var/stash/Library/Wallpaper").
             Add("INCLUDE", "/var/stash/usr/include").
             Add("LIBEXEC", "/var/stash/usr/libexec").
             Add("SHARE", "/var/stash/usr/share").
             Add("DARWIN9", "/var/stash/usr/arm-apple-darwin9");

        public static Action<string> OnJailbreakReported
        {
            get { return Cryoprison.Reporter.OnJailbreakReported; }
            set { Cryoprison.Reporter.OnJailbreakReported = value; }
        }

        public static Action<string, Exception> OnExceptionReported
        {
            get { return Cryoprison.Reporter.OnExceptionReported; }
            set { Cryoprison.Reporter.OnExceptionReported = value; }
        }

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
