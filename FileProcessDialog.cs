/* The File Commander - окно вывода статуса действия
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using Xwt;

namespace fcmd
{
	/// <summary>Operation progress dialog</summary>
	public class FileProcessDialog : Window
	{
		public Label lblStatus = new Label { TextAlignment = Alignment.Center };
		public ProgressBar pbrProgress = new ProgressBar();
		public Button cmdCancel = new Button { Label = "CANSEL" };
		public VBox Layout = new VBox();

		/// <summary>Initialize FileProcessDialog with four-row label</summary>
		public FileProcessDialog()
		{
			Title = Localizator.GetString("FileProgressDialogTitle");
			cmdCancel.Label = Localizator.GetString("Cancel");
			//this.Decorated = false;
			Resizable = false;

			Layout.PackStart(lblStatus, true, true);
			Layout.PackStart(pbrProgress, true, true);
			Layout.PackStart(cmdCancel,false,false);
			Content = Layout;

			InitialLocation = WindowLocation.Manual;
		}

		/// <summary>Initialize FileProcessDialog with a custom widget inside</summary>
		/// <param name="ProgressBox">Link to the xwt widget, which should be displayed in the FileProcessDialog.</param>
		public FileProcessDialog(ref Widget ProgressBox)
		{
			Title = Localizator.GetString("FileProgressDialogTitle");
			cmdCancel.Label = Localizator.GetString("Cancel");
			Decorated = false;

			Layout.PackStart(ProgressBox, true, true);
			Layout.PackStart(cmdCancel, false, false);
			Content = Layout;
		}
	}
}
