The File Commander
====

A customizable and extendable cross-platform two-panel file manager - The File Commander.

The File Commander can be downloaded as sources (it's the preferable way) from this repository and then compiled, or can be downloaded as a 7z archive from [Yandex.Disk](https://yadi.sk/d/XNZXI4MkTucdB). Note that the FC requires .NET Framework 4/4.5 being installed (pre-installed on Windows 8 and available for Windows XP and later) or Mono Runtime. Users of Ubuntu/other Debian-based distributions are able to install File Commander from PPA:

    deb http://ppa.launchpad.net/keks9n/fcmd/ubuntu trusty main 
    deb-src http://ppa.launchpad.net/keks9n/fcmd/ubuntu trusty main 
  
Add these repositories, update APT cache and install the "fcmd" package.

Please note that the FC is in the early stage of development and has many bugs. We give no warranty on the safety of your data or installed software. Use this at your own risk.

The File Commander has Russian and English lanugage files, you may select your own preferred language in the Settings window.

New contributions are welcome! For full info, see the README-DEV.md file.

System requirements: Windows XP or newer, Linux or Mac; Microsoft .NET Framework v4.X or Mono Runtime 3.2+. GTK# 3 is required if you're using Linux or BSD environment.

##How to compile:

1. Get the sources by cloning the Git repo: `$ git clone https://github.com/atauenis/fcmd.git`
2. Compile the fcmd.sln solution by using msbuild, xbuild or their GUI shells - Visual Studio 2010+, MonoDevelop 4.0+ or Xamarin Studio. The build target configuration: Release, x86.
3. Copy the pre-built XWT DLLs from the "stolen-opensource-libs" directory to the "bin/Release" directory.
   *Note that the DLLs can be safely replaced by official binaries from https://github.com/mono/xwt, but currently (24 june 2014) Xamarin doesn't provide them.*
4. Go to the "bin/Release" subdirectory and run "fcmd.exe".
5. Everything is done! To launch File Commander later, create a shortcut to the "fcmd.exe" file and then use it.

##Why *nix edition of FC requires GTK 3.0
The XWT Widget Toolkit, the FC user interface library, supporting both popular versions of GIMP ToolKit Plus (GTK+), 2.x and 3.x. The latest, third release of GTK+, released few years ago, supporting many cool features like window transparency and GNOME Shell support. Also, XWT has some strange bugs, that appearing only with GTK 2 and which does not appear with GTK 3. The GTK+ 3 is pre-installed in all fresh releases of all common Linux distributions. It's the reason why FC uses GTK 3.0 as the backend on *nix systems.