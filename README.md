# Cryoprison

A mobile app jailbreak detector to be used in Xamarin.

Supports iOS and Android.

## Overview

The host application should instantiate a **JailbreakDetector** from the correct
Cryoprison platform library.  It is recommended that debug builds be configured to
be simulator-friendly.

## Initialization

```
    if (Cryoprison.Factory.IsSupported)
    {
        var env = Cryoprison.Factory.CreateEnvironment();

        env.Reporter.OnJailbreakReported = (id) =>
        {
			// Global hook for jailbreak reporting, e.g. to
			// forward the issue to analytics
            Console.WriteLine($"Jailbreak: {id ?? "<null>"}");
        };

        env.Reporter.OnExceptionReported = (message, exception) =>
        {
			// Global hook for exception occurrance in the
			// cryoprison, e.g. to forward the exception to
			// analytics
            Console.WriteLine($"Jailbreak Error: {message}");
            Console.WriteLine(exception.ToString());
        };

		bool? simulatorFriendly = null;
#if DEBUG
		simulatorFriendly = true;
#endif
        this.jailbreakDetector = Cryoprison.Factory.CreateJailbreakDetector(env, simulatorFriendly);

		// it is recommended that the app query the library to determine
		// if it is jailbroken, e.g.
        var isJailbroken = this.jailbreakDetector.IsJailbroken;
        
		// the app can query to determine what jailbreaks were detected using
		// the violations list. This may indicate what tool was detected, but
		// many tools use similar methodologies so this cannot be gauranteed.
		// e.g.
        var violations = this.jailbreakDetector.Violations;
    }
```


## Usage

The **JailbreakDetector** implements the **IJailbreakDetector** interface, which
provides three methods, as described below:

Method | Description
------ | -----------
IsJailbroken | Runs the jailbreak detection code if neccessary, and returns true if any jailbreaks are detected.
Violations | Runs the jailbreak detection code if neccessary, and returns a list of all jailbreaks that are detected.
Reset | Resets the violations list, causing the next call to *IsJailbroken* or *Violations* to re-run the detection code.

Jailbreaks are detected by running **Inspectors**, which derive from the **IInspector**
interface.  When possible, Inspectors are implemented in a platform independent
fashion in the Cryoprison.Inspectors namespace.  Some Inspectors are however
platform dependent and are implemented in the PlatformSpecific namespaces of
their OS specific Cryoprison.

Inspectors perform checks, which are configured on a per-platform basis in the
platform-specific JailbreakDetector implementations using the **Checks** class.

The following checks are currently supported:

Method | Platform | Description
------ | -------- | -----------
DirectoryNotPresent | * | Checks that the configured directory is not present.
FileNotAccessible | * | Checks that the configured file is not readable.
FileNotDestructivelyWritable | * | Checks that the configured file can not be created by the app.  **Destroys any previous version of the file as a side effect.**
FileNotPresent | * | Checks that the configured file is not present.
PathNotSymbolicLink | * | Checks that the configured path is not as symbolic link.
UrlNotOpenable | * | Checks that the configured url can not be opened.
ShouldBeMobileProvisioned | iOS | Checks that the app has a mobile provisioning profile.
ShouldNotBeAbleToLocateFile | Android | Checks that a specific file is not found via 'which'.
ShouldNotHavePackageInstalled | Android | Check that a specific package is not installed.
ShouldNotHavePropValues | Android | Check for specific prop values via 'getprop'
ShouldNotHaveSpecificBuildTags | Android | Check that the kernal was not built with specific build tags, indicating a third party build.

The host may optionally register with the **Reporter** for global exception handling
and jailbreak detection, for example for logging purposes.  This is done
indirectly via assigning callback methods to the static callbacks found in the
JailbreakDetector class:

Method | Description
------ | -----------
OnJailbreakReported | Invoked each time a jailbreak is detected, with the ID of the jailbreak.
OnExceptionReported | Invoked each time an exception occurs, with an internal reason and the exception body

## NugetTest

The nuget test app is a very simple Xamarin forms application that performs a
jailbreak detection check when the main window appears.  It references the
Cryoprison library as a nuget package.  This is the preferred
method of using Cryoprison.



## Library

The Cryoprison library resides in this folder, and can be included
via manual means in your project.  Alternatively, use the Nuget package.

## Cryoprison

The Cryoprison nuget package resides in this folder.


## SampleApp

The sample app is a very simple Xamarin forms application that performs a
jailbreak detection check when the main window appears.  It references the
Cryoprison library directly as an assembly.  This is no longer the preferred
method of using Cryoprison.


