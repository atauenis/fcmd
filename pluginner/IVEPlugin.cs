/* The File Commander - plugin API
 * Viewer&Editor plugins' interface
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pluginner
{
    /// <summary>Interface for FC Viewer/Editor plugins</summary>
    public interface IVEPlugin : IPlugin
    {
        /// <summary>Open the file</summary>
        /// <param name="url">URL of the file</param>
        /// <param name="fsplugin">The FS plugin, which be used to access the file</param>
        void OpenFile(string url, pluginner.IFSPlugin fsplugin);
        
        /// <summary>Save the current file</summary>
        /// <param name="SaveAs">(optional) Save with new URL ("save as").</param>
        void SaveFile(bool SaveAs = false);

        /// <summary>Execute command in the plugin</summary>
        /// <param name="Command">The command's name</param>
        /// <param name="Arguments">The command's arguments (cmd line arguments)</param>
        void ExecuteCommand(string Command, string[] Arguments);

        /// <summary>The body of the displayed part of the plugin</summary>
        Xwt.Widget Body {get;}

        /// <summary>The Xwt menu for configuring file decoding mode in FCVE's menu "Format"</summary>
        Xwt.Menu FormatMenu{get;}

        /// <summary>Disables content editing in the plugin..</summary>
        bool ReadOnly { get;  set; }

        /// <summary>Defines, can the plugin edit files (set to false if the plugin is readonly)</summary>
        bool CanEdit { get; }
    }
}
