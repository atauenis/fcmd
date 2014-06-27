The File Commander
====

A customizable and extendable cross-platform two-panel file manager - The File Commander.

The File Commander can be downloaded as sources (it's the preferable way) from this repository and then compiled, or can be downloaded as a 7z archive from [Yandex.Disk](https://yadi.sk/d/XNZXI4MkTucdB). Note that the FC requires .NET Framework 4/4.5 being installed (pre-installed on Windows 8 and available for other version) or Mono Runtime. Users of Ubuntu/other Debian-based distributions are able to install File Commander from PPA:

    deb http://ppa.launchpad.net/keks9n/fcmd/ubuntu trusty main 
    deb-src http://ppa.launchpad.net/keks9n/fcmd/ubuntu trusty main 
  
Add these repositories, update APT cache and install the "fcmd" package.

Please note that the FC is in the early stage of development and has many bugs. We give no warranty on the safety of your data or installed software. Use this at your own risk.

At the moment FC contains only russian language file. If you're searching for an english version of FC, stop it. For now it doesn't exist and it will not be implemented in the near future (the string list is unstable, it gets updates almost every commit)

##How to compile:

1. Get the sources by cloning the Git repo: `$ git clone https://github.com/atauenis/fcmd.git`
2. Compile the fcmd.sln solution by using msbuild, xbuild or their GUI shells - Visual Studio 2010+, MonoDevelop 4.0+ or Xamarin Studio. The build target configuration: Release, x86.
3. Copy the pre-built XWT DLLs from the "stolen-opensource-libs" directory to the "bin/Release" directory.
   *Note that the DLLs can be safely replaced by official binaries from https://github.com/mono/xwt, but currently (24 june 2014) Xamarin doesn't provide them.*
4. Go to the "bin/Release" subdirectory and run "fcmd.exe".
5. Everything is done! To launch File Commander later, create a shortcut to the "fcmd.exe" file and then use it.

##Debugging SEGFAULTs
If FC crashes on Linux with a segmentation fault at a Cairo call, you should upgrade your distro to an version with Mono 3.2 and Cairo 2.13. For details, see https://github.com/mono/xwt/issues/323.
If a segfault has been thrown on other native calls and your PC RAM is not corrupt, try to run the `mono fcmd.exe` again for 10-20 times. There is a chance of success. :-) But a better way is to change your Linux distribution.

New contributions are welcomed! For full info, see the README-DEV.md file.

System requirements: Windows XP or newer, Linux or Mac; Microsoft .NET Framework v4.X or Mono Runtime 3.2+. GTK# is required for Linux.
