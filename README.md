The File Commander (fcmd)
====

A customizable and extendable cross-platform two-panel file manager - The File Commander.

##How to prepare to use:

1. Get the sources by cloning the Git repo: `$ git clone https://github.com/atauenis/fcmd.git`
2. Using the Visual Studio 2010 (or newer) or MonoDevelop 3.0 (or newer) compile the solution file.
3. Copy the pre-built XWT DLLs from the "stolen-opensource-libs" directory to the "bin/Debug" dir.
   *(note that the DLLs can be safely replaced by official binaries from https://github.com/mono/xwt, but currently (04/04/2014) Xamarin doesn't provide they).*
4. Go to the bin/Release (or bin/Debug) subdirectory and run "fcmd.exe".
5. All ready! To launch File Commander later, create a shortcut to the "fcmd.exe" file and then use it.

System requirements: Windows XP+, Linux or Mac; Microsoft .NET Framework v4.X or Mono Runtime 3.0+.
Also you need a C# 4.0 compiler like Visual Studio's csc, or MonoDevelop/Xamarin Studio's mcs.