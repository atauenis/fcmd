The File Commander
====

A customizable and extendable cross-platform two-panel file manager - The File Commander.

The File Commander can be downloaded as sources (it's the preferred way) from this repository and then compiled, or can be downloaded as an 7z archive from [Yandex.Disk](https://yadi.sk/d/XNZXI4MkTucdB). Note that the FC requires installed .NET Framework or Mono Runtime.

Please note that the FC is in early stage on development and has many bugs. We giving no warranties about the safety of your data or installed software. Use at your own risk.

##How to compile:

1. Get the sources by cloning the Git repo: `$ git clone https://github.com/atauenis/fcmd.git`
2. Using the Visual Studio 2010 (or newer) or MonoDevelop 3.0 (or newer) compile the solution file.
3. Copy the pre-built XWT DLLs from the "stolen-opensource-libs" directory to the "bin/Release" directory.
   *Note that the DLLs can be safely replaced by official binaries from https://github.com/mono/xwt, but currently (16 june 2014) Xamarin doesn't provide they.*
4. Go to the "bin/Release" subdirectory and run "fcmd.exe".
5. All are completed! To launch File Commander later, create a shortcut to the "fcmd.exe" file and then use it.

New contributions to the project are welcome! For full info, see the README-DEV.md file.

System requirements: Windows XP or newer, Linux or Mac; Microsoft .NET Framework v4.X or Mono Runtime 3.0+. On Linux there is required GTK#.
