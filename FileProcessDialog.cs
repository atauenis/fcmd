/* The File Commander - окно вывода статуса действия
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;

namespace fcmd
{
    /// <summary>Operation progress dialog</summary>
    public class FileProcessDialog : Xwt.Window
    {
        public Xwt.Label lblStatus = new Xwt.Label() { TextAlignment = Xwt.Alignment.Center };
        public Xwt.ProgressBar pbrProgress = new Xwt.ProgressBar();
        public Xwt.Button cmdCancel = new Xwt.Button() { Label = "CANSEL" };
        public Xwt.VBox Layout = new Xwt.VBox();

        /// <summary>Initialize FileProcessDialog with four-row label</summary>
        public FileProcessDialog()
        {
            Localizator locale = new Localizator();
            this.Title = locale.GetString("FileProgressDialogTitle");
            cmdCancel.Label = locale.GetString("Cancel");
            //this.Decorated = false;
            this.Resizable = false;

            Layout.PackStart(lblStatus, true, true);
            Layout.PackStart(pbrProgress, true, true);
            Layout.PackStart(cmdCancel,false,false);
            this.Content = Layout;

            this.InitialLocation = Xwt.WindowLocation.Manual;
        }

        /// <summary>Initialize FileProcessDialog with a custom widget inside</summary>
        /// <param name="ProgressBox">Link to the xwt widget, which should be displayed in the FileProcessDialog.</param>
        public FileProcessDialog(ref Xwt.Widget ProgressBox)
        {
            Localizator locale = new Localizator();
            this.Title = locale.GetString("FileProgressDialogTitle");
            cmdCancel.Label = locale.GetString("Cancel");
            this.Decorated = false;

            Layout.PackStart(ProgressBox, true, true);
            Layout.PackStart(cmdCancel, false, false);
            this.Content = Layout;
        }
    }
}
