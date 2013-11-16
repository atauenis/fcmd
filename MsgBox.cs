/* The File Commander backend   Ядро File Commander
 * Abstract message box         Абстрактный message box
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcmd{
    /// <summary>
    /// Show system message box
    /// </summary>
    class MsgBox{
        public enum MsgBoxType{
            /// <summary>
            /// No icon
            /// </summary>
            Simple,

            /// <summary>
            /// With information icon (i)
            /// </summary>
            Information,

            /// <summary>
            /// With warning icon /!\
            /// </summary>
            Warning,

            /// <summary>
            /// With failure error (x)
            /// </summary>
            Error
        }

        /// <summary>
        /// Shows a simple Message Box
        /// </summary>
        /// <param name="text">The information in the msgbox</param>
        public MsgBox(string text){
            new MsgBox(text, "", MsgBoxType.Simple);
        }

        /// <summary>
        /// Show the defined Message Box
        /// </summary>
        /// <param name="text">The dialog text</param>
        /// <param name="title">The dialog title</param>
        /// <param name="dialogtype">The dialog type (error, info, etc)</param>
        public MsgBox(string text, string title, MsgBoxType dialogtype){
            //initialize xwt
            switch (Environment.OSVersion.Platform){
                case PlatformID.Win32NT:
                    Xwt.Application.InitializeAsGuest(Xwt.ToolkitType.Wpf);
                    break;
                case PlatformID.MacOSX: //i don't sure that Mono detect OSX as OSX, not Unix; see http://mono.wikia.com/wiki/Detecting_the_execution_platform
                    Xwt.Application.InitializeAsGuest(Xwt.ToolkitType.Cocoa);
                    break;
                default:
                case PlatformID.Unix: //gtk fallback for unknown oses
                    Xwt.Application.InitializeAsGuest(Xwt.ToolkitType.Gtk);
                    break;
            }

            //show the msgbox
            switch (dialogtype){
                case MsgBoxType.Information:
                    Xwt.MessageDialog.ShowMessage(text, title);
                    break;
                case MsgBoxType.Warning:
                    Xwt.MessageDialog.ShowWarning(text, title);
                    break;
                case MsgBoxType.Error:
                    Xwt.MessageDialog.ShowError(text, title);
                    break;
                case MsgBoxType.Simple:
                    //this is windows only
#if Win
                    System.Windows.Forms.MessageBox.Show(text, title);
#else
                    Xwt.MessageDialog.ShowMessage(text, title);
#endif
                    break;
            }
        }
    }
}
