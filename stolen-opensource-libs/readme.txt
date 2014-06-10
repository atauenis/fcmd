In this folder is placed some compiled open source DLLs
of the Xamarin XWT Widget Toolkit (github.com/mono/xwt)
Xwt.dll - the XWT Window Toolkit
Xwt.Gtk.dll - the XWT GTK backend
Xwt.Gtk.dll.config - DLL/SO remapings for GTK backend
Xwt.Gtk.Windows.dll - GTK optimizations for Win32
Xwt.WPF.dll - the XWT WPF backend
(C) Xamarin corporation and third-party contributors.

The files are updated manually (Xamarin doesn't provide
up-to-date binaries, so the DLLs are compiled by A.T.).
The latest version of the XWT DLLs can be compiled from
the project's Git repository: https://github.com/mono/xwt
at any time. The newer binaries should work same as these.

XWT is main UI toolkit in the File Commander and all of
it's plugins. It feels natively on any OS (Windows,
Linux/GTK, MacOS X) and allows embedding of system native
controls (i.e. WPF controls on Win32 or GTK widgets on *nix).

Also here is placed files of μCSS (mucss.dll & mucss.dll.pdb),
the File Commander CSS parser:https://github.com/atauenis/mucss