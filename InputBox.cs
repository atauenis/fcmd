/* The File Commander - InputBox
 * The dialog box for asking the user (like VBA InputBox)
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
namespace fcmd{
	/// <summary>The dialog box for asking the user</summary>
	public class InputBox : Xwt.Dialog{
		Xwt.Label lblQuestion = new Xwt.Label();
		Xwt.TextEntry txtAnwser = new Xwt.TextEntry();
		Xwt.VBox box = new Xwt.VBox();
		public Xwt.Table OtherWidgets = new Xwt.Table();


		public InputBox(string AskText)
		{
			lblQuestion.Text = AskText;

			this.Buttons.Add(Xwt.Command.Ok);
			this.Buttons.Add(Xwt.Command.Cancel);
		}

		public InputBox(string AskText, string DefaultValue)
		{
			lblQuestion.Text = AskText;
			txtAnwser.Text = DefaultValue;

			this.Buttons.Add(Xwt.Command.Ok);
			this.Buttons.Add(Xwt.Command.Cancel);
		}

		public InputBox(string AskText, string DefaultValue, Xwt.Command[] Buttons)
		{
			lblQuestion.Text = AskText;
			txtAnwser.Text = DefaultValue;
			
			this.Buttons.Add(Buttons); 
		}

		/// <summary>Shows dialog</summary>
		/// <returns><value>True</value> if user want to proceed current operation, and <value>False</value> if user don't.</returns>
		public bool ShowDialog(Xwt.WindowFrame parent = null)
		{
			Build();
            Xwt.Command DialogResult = null;
            if (parent == null){
                DialogResult = this.Run();
            }
            else{
                DialogResult = this.Run(parent);
            }
            //4beginners: xwtdialog.Run() == winform.ShowDialog() == vb6form.Show(vbModal);

			if (DialogResult == null) return false;
			switch (DialogResult.Id) //hack due to C# limitation (it's unable to do switch(){} on custon types) 
			{
				case "Add":
				case "Apply":
				case "Clear":
				case "Copy":
				case "Cut":
				case "Delete":
				case "Ok":
				case "Paste":
				case "Remove":
				case "Save":
				case "SaveAs":
				case "Yes":
					return true;
			}
			return false;
		}

		/// <summary>Builds the InputBox dialog</summary>
		private void Build()
		{
			box.PackStart(lblQuestion);
			box.PackStart(txtAnwser);
			if(OtherWidgets.Placements.Count > 0)
			box.PackStart(OtherWidgets);
			this.Content = box;
			this.ShowInTaskbar = false;
			this.Resizable = false;
			this.Title = System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString();
			this.CloseRequested += (o, ea) => { this.Hide(); };
			foreach (Xwt.DialogButton dbtn in this.Buttons)
			{
				dbtn.Clicked += (o, ea) => { this.Hide(); };
			}
		}

		/// <summary>Which text user entered</summary>
		public string Result{
			get{return txtAnwser.Text;}
		}

		/// <summary>What button user clicked</summary>
		public Xwt.Command Command{
			get { return this.Command; }
		}
	}
}
