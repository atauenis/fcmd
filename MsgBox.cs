/* The File Commander backend   Ядро File Commander
 * Abstract message box		 Абстрактный message box
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using pluginner.Toolkit;

namespace fcmd{
	/// <summary>
	/// Show system message box
	/// </summary>
	[Obsolete("Obsolete due to architecture change at the autumn of 2013. Use Xwt MessageDialog.Show*** and Pluginner.Utilities.Show*** instead.")]
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
			var toolkitType = OSVersionEx.GetToolkitType();
			Xwt.Application.InitializeAsGuest(toolkitType);

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
					Xwt.MessageDialog.ShowMessage(text, title);
					break;
			}
		}
	}
}
