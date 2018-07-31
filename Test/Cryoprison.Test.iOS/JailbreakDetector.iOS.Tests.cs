using System;
using Xunit;
using Cryoprison.iOS;
using System.Linq;
using System.IO;
using Cryoprison.Inspectors;
using Foundation;

namespace Cryoprison.Test.iOS
{
    public class JailbreakDetectorAndroidTests
    {
        [Fact]
        public void ShouldClaimThatTheDeviceIsJailbrokenByDefault()
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Fact]
        public void ShouldNotClaimThatTheDeviceIsJailbrokenIfMobileProvisionIsEmbedded()
        {
            using (var gel = new GlobalLock())
            {
                Foundation.NSBundle.MainBundle.MockPathForResource = (a, b) =>
                {
                    if (a == "embedded" && b == "mobileprovision")
                    {
                        return "found";
                    }
                    return null;
                };

                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                Assert.False(jbd.IsJailbroken);
            }
        }

        /* CYDIA */
        [Theory]
        [InlineData("/private/var/lib/cydia")]
        [InlineData("/var/lib/cydia")]
        [InlineData("/usr/libexec/cydia")]
        public void ShouldDetectIfCydiaLibDirectoryIsPresent(string directory)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.Directory.Exists = (path) =>
                {
                    return path == directory;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("/private/var/stash")]
        [InlineData("/private/var/db/stash")]
        public void ShouldDetectIfCydiaStashDirectoryIsPresent(string directory)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.Directory.Exists = (path) =>
                {
                    return path == directory;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("cydia:")]
        public void ShouldDetectIfCydiaUrlCanBeOpened(string scheme)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                UIKit.UIApplication.SharedApplication.MockCanOpenUrl = (url) =>
                {
                    return url.MockUrl.StartsWith(scheme);
                };

               Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("/Applications/Cydia.app")]
        public void ShouldDetectIfCydiaAppIsInstalled(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("/private/var/tmp/cydia.log")]
        [InlineData("/System/Library/LaunchDaemons/com.saurik.Cydia.Startup.plist")]
        public void ShouldDetectIfCydiaAuxilliaryFilesAreInstalled(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("/Library/MobileSubstrate/MobileSubstrate.dylib")]
        [InlineData("/Library/MobileSubstrate/DynamicLibraries/xCon.dylib")]
        public void ShouldDetectIfCydiaSubstrateFilesAreInstalled(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* Various apps */
        [Theory]
        [InlineData("/Applications/blackra1n.app")]
        [InlineData("/Applications/FakeCarrier.app")]
        [InlineData("/Applications/MxTube.app")]
        [InlineData("/Applications/RockApp.app")]
        [InlineData("/Applications/Snoop-it Config.app")]
        [InlineData("/Applications/WinterBoard.app")]
        [InlineData("/Applications/Icy.app")]
        public void ShouldDetectIfAppIsInstalled(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* Spirit */
        [Theory]
        [InlineData("/var/mobile/Media/spirit")]
        public void ShouldDetectIfSpiritIsInstalled(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* iOS.Ikee worm */
        [Theory]
        [InlineData("/System/Library/LaunchDaemons/com.ikey.bbot.plist")]
        public void ShouldDetectIfIKeeWormPresent(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* APT - Advanced Packaging Tool */
        [Theory]
        [InlineData("/private/var/cache/apt")]
        [InlineData("/var/lib/apt")]
        [InlineData("/private/var/lib/apt")]
        [InlineData("/private/etc/apt")]
        [InlineData("/etc/apt")]
        [InlineData("/private/cache/apt")]
        [InlineData("/private/etc/dpkg/origins/debian")]
        public void ShouldDetectIfAptDirectoryIsPresent(string directory)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.Directory.Exists = (path) =>
                {
                    return path == directory;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("/etc/apt")]
        public void ShouldDetectIfAptConfigDirectoryExistsAsFile(string file)
        {
            // Some tools seem to test for this, not sure whether it is needed.
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* SBSettings */
        [Theory]
        [InlineData("/Applications/SBSettings.app")]
        public void ShouldDetectIfSBSettingsAppIsInstalled(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        [Theory]
        [InlineData("/private/var/mobileLibrary/SBSettingsThemes")]
        public void ShouldDetectIfSBSettingsThemesDirectoryIsPresent(string directory)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.Directory.Exists = (path) =>
                {
                    return path == directory;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* Shells */
        [Theory]
        [InlineData("/bin/bash")]
        [InlineData("/bin/sh")]
        public void ShouldDetectIfShellIsAccessible(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* SSH client, server or tools + SFTP */
        [Theory]
        [InlineData("/usr/bin/sshd")]
        [InlineData("/usr/sbin/sshd")]
        [InlineData("/usr/bin/ssh")]
        [InlineData("/private/etc/ssh/sshd_config")]
        [InlineData("/etc/ssh/sshd_config")]
        [InlineData("/usr/libexec/ssh-keysign")]
        [InlineData("/usr/libexec/sftp-server")]
        public void ShouldDetectIfSshIsAccessible(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* Sandbox violations */
        [Theory]
        [InlineData("/private/cryoprison")]
        public void ShouldDetectIfCanWriteToPrivate(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Open = (path, mode, access, share) =>
                {
                    if (path != file)
                    {
                        throw new System.IO.IOException();
                    }

                    var writables = new FileMode[] { FileMode.Append, FileMode.Create, FileMode.CreateNew, FileMode.OpenOrCreate, FileMode.Truncate };
                    if (!writables.Any(x => x == mode))
                    {
                        throw new System.IO.IOException();
                    }

                    if (access != FileAccess.Read)
                    {
                        throw new System.IO.IOException();
                    }

                    return new MemoryStream();
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* syslog */
        [Theory]
        [InlineData("/var/log/syslog")]
        [InlineData("/private/var/log/syslog")]
        public void ShouldDetectIfSysLogIsPresent(string file)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.File.Exists = (path) =>
                {
                    return path == file;
                };

                Assert.True(jbd.IsJailbroken);
            }
        }

        /* Symbolic Links */
        [Theory]
        [InlineData("/Applications")]
        [InlineData("/var/stash/Library/Ringtones")]
        [InlineData("/var/stash/Library/Wallpaper")]
        [InlineData("/var/stash/usr/include")]
        [InlineData("/var/stash/usr/libexec")]
        [InlineData("/var/stash/usr/share")]
        [InlineData("/var/stash/usr/arm-apple-darwin9")]
        public void ShouldDetectIfImportantDirectoryHasBecomeASymLink(string directory)
        {
            using (var gel = new GlobalLock())
            {
                var jbd = new Cryoprison.iOS.JailbreakDetector(gel.Env, simulatorFriendly: false);

                gel.Env.System.IO.Directory.Exists = (path) =>
                {
                    return directory == path;
                };

                Foundation.NSFileManager.DefaultManager.MockGetAttributes = GetAttributesReturnsSymLink;

                Assert.True(jbd.IsJailbroken);
            }
        }

        private Foundation.NSFileAttributes GetAttributesReturnsSymLink(string path, out Foundation.NSError error)
        {
            error = new NSError();

            return new NSFileAttributes
            {
                Type = NSFileType.SymbolicLink
            };
        }
    }
}
