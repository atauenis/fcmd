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
		Xwt.Table Layout = new Xwt.Table();
		Xwt.Label lblAsk = new Xwt.Label();
		Xwt.Button cmdReplace = new Xwt.Button();
		Xwt.Button cmdReplaceAll = new Xwt.Button();
		Xwt.Button cmdReplaceOld = new Xwt.Button();
		Xwt.Button cmdSkip = new Xwt.Button();
		Xwt.Button cmdSkipAll = new Xwt.Button();
		Xwt.Button cmdCompare = new Xwt.Button {Sensitive=false};
		public ClickedButton ChoosedButton;

        /// <summary>Initialize RPD. Please be careful with threads - run only in the UI thread! Otherwise there will be bugs</summary>
        /// <param name="filename">The both files' name</param>
		public ReplaceQuestionDialog (string filename)
		{
            /* Why the warning about threads? When calling the RPD form thread, different than that where it's created,
             * an exception throws (due to illegal cross-thread call). If the RPD is created in a thread,
             * that is not the program's main thread (UI thread), the RPD works, but the window's or widgets'
             * sizes may be (and, at many times, are) invalid. To prevent this, the best practice is
             * to create & use the RPD instances only in the UI thread. A.T. 14 jun 2014. */

			this.Content = Layout;
			Layout.Add(cmdReplace,0,1);
			Layout.Add(cmdReplaceAll,0,2);
			Layout.Add(cmdSkip,1,1);
			Layout.Add(cmdSkipAll,1,2);
			Layout.Add(cmdReplaceOld,2,2);
			Layout.Add(cmdCompare,2,1);
			Layout.Add(lblAsk,0,0,1,3);
			this.Buttons.Add(Xwt.Command.Cancel);

			Title = Localizator.GetString("ReplaceQDTitle");
			lblAsk.Text = String.Format(Localizator.GetString("ReplaceQDText"), filename);
			cmdReplace.Label = Localizator.GetString("ReplaceQDReplace");
			cmdReplaceAll.Label = Localizator.GetString("ReplaceQDReplaceAll");
			cmdReplaceOld.Label = Localizator.GetString("ReplaceQDReplaceOld");
			cmdSkip.Label = Localizator.GetString("ReplaceQDSkip");
			cmdSkipAll.Label = Localizator.GetString("ReplaceQDSkipAll");
			cmdCompare.Label = Localizator.GetString("ReplaceQDCompare");

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

