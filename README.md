The File Commander (fcmd)
====

A customizable and extendable cross-platform two-panel file manager - The File Commander.

##How to compile:

1. Get the sources by cloning the Git repo: $ git clone https://github.com/atauenis/fcmd.git
2. Using the Visual Studio 2010 (or newer) or MonoDevelop 3.0 (or newer) compile the solution file.
3. Copy the modXWT DLLs from the "stolen-opensource-libs" directory to the bin/Debug dir.
   or build your own binaries (sources can be downloaded from https://github.com/atauenis/xwt, branch "modxwt").
4. Go to the bin/Release (or bin/Debug) subdirectory and run "fcmd.exe".

System requirements: Windows XP+ or Linux (Mac isn't supported), Microsoft .NET Framework v4.x or a fresh Mono Runtime version.
Also you need a C# 4.0 compiler.