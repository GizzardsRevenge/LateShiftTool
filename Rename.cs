using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LateShiftTool
{
    public partial class Rename : Form
    {
        private string _shortFilename;

        public Rename(Video.MovieSection section, string shortName, string currentName)
        {
            InitializeComponent();

            _shortFilename = shortName;

            txtSection.Text = section.ToString();
            txtPreviousName.Text = currentName;
            txtNewName.Text = String.Empty;
            DialogResult = DialogResult.Cancel;
        }

        private void txtNewName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                HandleOK();
            }
        }

        public string NewName
        {
            get { return txtNewName.Text; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            HandleOK();
        }

        private void HandleOK()
        {
            if (String.IsNullOrWhiteSpace(txtNewName.Text))
            {
                MessageBox.Show("Illegal name");
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
