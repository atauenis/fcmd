/* The File Commander
 * The "The file xxx already exists. Replace? Replace all? Skip? Skip all?" dialog window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;

namespace fcmd
{
	public class ReplaceQuestionDialog : Xwt.Dialog
	{
		Localizator Locale = new Localizator();
		Xwt.Table Layout = new Xwt.Table();
		Xwt.Label lblAsk = new Xwt.Label();
		Xwt.Button cmdReplace = new Xwt.Button();
		Xwt.Button cmdReplaceAll = new Xwt.Button();
		Xwt.Button cmdReplaceOld = new Xwt.Button();
		Xwt.Button cmdSkip = new Xwt.Button();
		Xwt.Button cmdSkipAll = new Xwt.Button();
		Xwt.Button cmdCompare = new Xwt.Button(){Sensitive=false};
		public ClickedButton ChoosedButton;

		public ReplaceQuestionDialog (string filename)
		{
			this.Content = Layout;
			Layout.Add(cmdReplace,0,1);
			Layout.Add(cmdReplaceAll,0,2);
			Layout.Add(cmdSkip,1,1);
			Layout.Add(cmdSkipAll,1,2);
			Layout.Add(cmdReplaceOld,2,2);
			Layout.Add(cmdCompare,2,1);
			Layout.Add(lblAsk,0,0,1,3);
			this.Buttons.Add(Xwt.Command.Cancel);

			Title = Locale.GetString("ReplaceQDTitle");
			lblAsk.Text = String.Format(Locale.GetString("ReplaceQDText"), filename);
			cmdReplace.Label = Locale.GetString("ReplaceQDReplace");
			cmdReplaceAll.Label = Locale.GetString("ReplaceQDReplaceAll");
			cmdReplaceOld.Label = Locale.GetString("ReplaceQDReplaceOld");
			cmdSkip.Label = Locale.GetString("ReplaceQDSkip");
			cmdSkipAll.Label = Locale.GetString("ReplaceQDSkipAll");
			cmdCompare.Label = Locale.GetString("ReplaceQDCompare");

			cmdReplace.Clicked += (o,ea) => { Choose(ClickedButton.Replace); };
			cmdReplaceAll.Clicked += (o,ea) => { Choose(ClickedButton.ReplaceAll); };
			cmdReplaceOld.Clicked += (o,ea) => { Choose(ClickedButton.ReplaceOld); };
			cmdSkip.Clicked += (o,ea) => { Choose(ClickedButton.Skip); };
			cmdSkipAll.Clicked += (o,ea) => { Choose(ClickedButton.SkipAll); };
			Buttons[0].Clicked += (o,ea) => { Choose(ClickedButton.Cancel); };
		}

		public enum ClickedButton{
			Replace, ReplaceAll, ReplaceOld, Skip, SkipAll, Cancel
		}

		public new ClickedButton Run()
		{
			base.Run();
			return ChoosedButton;
		}

		private void Choose(ClickedButton cb)
		{
			ChoosedButton = cb;
			this.OnCommandActivated(Xwt.Command.Ok);
			this.Hide();
		}
	}
}

