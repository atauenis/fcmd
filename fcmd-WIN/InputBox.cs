using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace fcmd{
    [Obsolete("Use Xwt.MessageDialog.AskQuestion instead")]
    public partial class InputBox : Form{
        public InputBox(string AskText){
            InitializeComponent();
            lblQuestion.Text = AskText;
        }

        public InputBox(string AskText, string DefaultValue){
            InitializeComponent();
            lblQuestion.Text = AskText;
            txtAnwser.Text = DefaultValue;
        }

        public string Result{
            get{
                return txtAnwser.Text;
            }
        }
    }
}
