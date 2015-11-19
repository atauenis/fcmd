The F Commander
====

A customizable and extendable cross-platform two-panel file manager - the F Commander.

The F Commander can be downloaded as sources (it's the preferable way) from this repository and then compiled, or can be downloaded as a 7z archive from [Yandex.Disk](https://yadi.sk/d/XNZXI4MkTucdB). Note that the FC requires .NET Framework 4/4.5 being installed (pre-installed on Windows 8 and available for Windows XP and later) or Mono Runtime. Users of Ubuntu/other Debian-based distributions are able to install F Commander from PPA:

    deb http://ppa.launchpad.net/keks9n/fcmd/ubuntu trusty main 
    deb-src http://ppa.launchpad.net/keks9n/fcmd/ubuntu trusty main 
  
Add these repositories, update APT cache and install the "fcmd" package.

Please note that the FC is in the early stage of development and has many bugs. We give no warranty on the safety of your data or installed software. Use this at your own risk.

The F Commander has Russian and English lanugage files, you may select your own preferred language in the Settings window.

New contributions are welcome! For full info, see the README-DEV.md file.

System requirements: Windows XP or newer, Linux or Mac; Microsoft .NET Framework v4.X or Mono Runtime 3.2+. GTK# 3 is required if you're using Linux or BSD environment.

##How to compile:

1. Get the sources by cloning the Git repo: `$ git clone https://github.com/atauenis/fcmd.git`
2. Compile the fcmd.sln solution by using msbuild, xbuild or their GUI shells - Visual Studio 2012+, MonoDevelop 4.0+ or Xamarin Studio. The build target configuration: Release, x86.
3. Copy the pre-built XWT DLLs from the "stolen-opensource-libs" directory to the "bin/Release" directory.
   *Note that the DLLs can be safely replaced by official binaries from https://github.com/mono/xwt, but currently (24 june 2014) Xamarin doesn't provide them.*
4. Go to the "bin/Release" subdirectory and run "fcmd.exe".
5. Everything is done! To launch F Commander later, create a shortcut to the "fcmd.exe" file and then use it.

##Why *nix edition of F Commander require GTK 3.0
The XWT Widget Toolkit, the F Commander user interface library, supporting both popular versions of GIMP ToolKit Plus (GTK+), 2.x and 3.x. The latest, third release of GTK+, released few years ago, supporting many cool features like window transparency and GNOME Shell support. Also, XWT has some strange bugs, that appearing only with GTK 2 and which does not appear with GTK 3. The GTK+ 3 is pre-installed in all fresh releases of all common Linux distributions. It's the reason why FC uses GTK 3.0 as the backend on *nix systems.

##Latest news
The project has been started in 2013 year as "File Commander". At that point, the File Commander's "idea" was to create a cross-platform "clone" of major two-panel file managers, such as Total Commander, Midnight Commander and FAR Manager. The developing did go very slow and at the autumn of 2014 year has stopped entirely.
On November, 2015 developing was continued, and the main developer, Alexander Tauenis, started a new branch - "fcmd2016". The architecture of FC will be significantly reworked. After completing the following 7 steps, the branch will be merged into "master" branch. I hope this will be done on January, 2016.

1. [ok; #28, #19] The main listing view widget is now FListView. It's an asynchronous listview-like widget that can show any kinds of data, but is optimized for displaying of file info "rows" (or "tiles").
2. [todo; #19] ListView2 and VirtualListView will be removed, and the FileListPanel will be rewrited from scratch
3. [todo; #27] FTP filesystem plugin will be removed
4. [todo; #27] Local FS plugin will be rewrited to completely asynchrous work
5. [todo] Plugin API (pluginner.IPlugin) will be rewrited to support bidirectional interaction between FC and plugins with using delegates and string-defined calls (instead of APICallHost, APICallPlugin & APICompatibility).
6. [todo] The Virtual Terminal Emulator in panels will be rewrited with using of text input events and a custom widget.
7. [todo] The name of the project will be changed to "F Commander" - it doesn't copying the name of the old OS/2 alternate file manager and corresponds with classic, keyboard-friendly user interface. But this is not the final decision, this is reason for this is the last item in this to-do list.

Please do not add any commits to this branch. The "fcmd2016" branch will be merged into "master" when major changes will be done. After this, the project will return to normal state and collective development can be continued.
Bugs #19, #27, #28 will be closed when the branch will be merged into "master".