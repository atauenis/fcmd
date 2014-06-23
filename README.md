The File Commander
====

A customizable and extendable cross-platform two-panel file manager - The File Commander.

The File Commander can be downloaded as sources (it's the preferred way) from this repository and then compiled, or can be downloaded as an 7z archive from [Yandex.Disk](https://yadi.sk/d/XNZXI4MkTucdB). Note that the FC requires installed .NET Framework 4/4.5 (pre-installed on Windows 8 and available for other version) or Mono Runtime. Users of Ubuntu/other Debian clones may install File Commander from PPA:

    deb http://ppa.launchpad.net/keks9n/fcmd/ubuntu trusty main 
    deb-src http://ppa.launchpad.net/keks9n/fcmd/ubuntu trusty main 
  
Add these repositories, update APT cache and install the "fcmd" package.

Please note that the FC is in the early stage of development and has many bugs. We giving no warranties about the safety of your data or installed software. Use at your own risk.

The FC currently contains only russian language file. If you're finding an english version of FC, do not continue the search. It is not present now. And it will not be implemented in the near future (until the locale string list is not updating almost every commit)

##How to compile:

1. Get the sources by cloning the Git repo: `$ git clone https://github.com/atauenis/fcmd.git`
2. Compile the fcmd.sln solution by using msbuild, xbuild or they's GUI shells - Visual Studio 2010+, MonoDevelop 4.0+ or Xamarin Studio. The destination configuration: Release, x86.
3. Copy the pre-built XWT DLLs from the "stolen-opensource-libs" directory to the "bin/Release" directory.
   *Note that the DLLs can be safely replaced by official binaries from https://github.com/mono/xwt, but currently (24 june 2014) Xamarin doesn't provide they.*
4. Go to the "bin/Release" subdirectory and run "fcmd.exe".
5. All are completed! To launch File Commander later, create a shortcut to the "fcmd.exe" file and then use it.

##Debugging SEGFAULTs
If FC crashes on Linux with a segmentation fault at a Cairo call, you should upgrade your distro to an version with Mono 3.2 and Cairo 2.13. For details, see https://github.com/mono/xwt/issues/323 .
If a segfault has been thrown on other native calls and your PC RAM is not corrupt, try re-running the `mono fcmd.exe` again for 10-20 times. One run may be successfull. :-) But a better way is to switch your distro to an other.

New contributions to the project are welcome! For full info, see the README-DEV.md file.

System requirements: Windows XP or newer, Linux or Mac; Microsoft .NET Framework v4.X or Mono Runtime 3.2+. On Linux there is required GTK#.
