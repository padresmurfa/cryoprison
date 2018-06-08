# Cryoprison

A Jailbreak detector for Xamarin.

## Architecture

The host application should instantiate a JailbreakDetector from the correct
Cryoprison platform library.

The JailbreakDetector implements the IJailbreakDetector interface, which
provides three methods, as described below:

Method | Description
------ | -----------
IsJailbroken | Runs the jailbreak detection code if neccessary, and returns true if any jailbreaks are detected.
Violations | Runs the jailbreak detection code if neccessary, and returns a list of all jailbreaks that are detected.
Reset | Resets the violations list, causing the next call to IsJailbroken or Violations to re-run the detection code.

Jailbreaks are detected by running Inspectors, which derive from the IInspector
interface.  When possible, Inspectors are implemented in a platform independent
fashion in the Cryoprison.Inspectors namespace.  Some Inspectors are however
platform dependent and are implemented in the PlatformSpecific namespaces of
their OS specific Cryoprison.

Inspectors perform checks, which are configured on a per-platform basis in the
platform-specific JailbreakDetector implementations.

The following checks are currently supported:

Method | Platform | Description
------ | -------- | -----------
DirectoryNotPresent | * | Checks that the configured directory is not present.
FileNotAccessible | * | Checks that the configured file is not readable.
FileNotDestructivelyWritable | * | Checks that the configured file can not be created by the app.  Destroys the previous version of the file, if present.
FileNotPresent | * | Checks that the configured file is not present.
PathNotSymbolicLink | * | Checks that the configured path is not as symbolic link.
UrlNotOpenable | * | Checks that the configured url can not be opened.
ShouldBeMobileProvisioned | iOS | Checks that the app has a mobile provisioning profile.

The host may optionally register with the Reporter for global exception handling
and jailbreak detection, for example for logging purposes.  This is done
indirectly via assigning callback methods to the static callbacks found in the
JailbreakDetector class:

Method | Description
------ | -----------
OnJailbreakReported | Invoked each time a jailbreak is detected, with the ID of the jailbreak.
OnExceptionReported | Invoked each time an exception occurs, with an internal reason and the exception body


## SampleApp

The sample app is a very simple xamarin forms application that performs a
jailbreak detection check when the main window appears.

## Library

The Cryoprison is implemented in a platform independent fashion as much as
possible.  