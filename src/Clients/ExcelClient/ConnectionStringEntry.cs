using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelClientUI
{
    public partial class ConnectionStringEntry : Form
    {
        public ConnectionStringEntry()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void ConnectionTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if(!Uri.TryCreate(ConnectionTextBox.Text, UriKind.Absolute, out Uri result))
                {
                    errorProvider1.SetError(ConnectionTextBox, "Not a valid connection");
                }
            }
            catch(Exception ex)
            {
                errorProvider1.SetError(ConnectionTextBox, ex.Message);
            }
        }

        private void ConnectionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!Uri.TryCreate(ConnectionTextBox.Text, UriKind.Absolute, out Uri result))
            {
                okButton.Enabled = false;
            }
            else
            {
                okButton.Enabled = true;
            }
        }
    }
}
