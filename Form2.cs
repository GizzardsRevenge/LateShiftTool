using System;
using System.Windows.Forms;

namespace LateShiftTool
{
    public partial class Form2 : Form
    {
        private Video.MovieSection _chosenSection = Video.MovieSection.Other;
        private string _chosenName = String.Empty;
        private const int kNumberOfRadios = 16;

        public Form2(Video.MovieSection initialSection, string shortName)
        {
            InitializeComponent();

            int numberOfItemsInEnum = Video.MovieSection.GetNames(typeof(Video.MovieSection)).Length;

            // Update the radio buttons
            for (int i = 0; i < kNumberOfRadios; i++)
            {
                if (i < numberOfItemsInEnum)
                {
                    groupBox1.Controls[i].Text = Enum.GetNames(typeof(Video.MovieSection))[i].ToString();
                }
                else
                {
                    groupBox1.Controls[i].Text = "--------";
                    groupBox1.Controls[i].Enabled = false;
                }
            }

            (groupBox1.Controls[(int)initialSection] as RadioButton).Checked = true;

            if (FriendlyNames.Exists(shortName))
                textBox1.Text = FriendlyNames.Lookup(shortName);
            else
                textBox1.Text = String.Empty;
        }

        public Video.MovieSection Section
        {
            get { return _chosenSection; }
        }

        public string NewName
        {
            get { return _chosenName; }
        }

        public string FriendlyName
        {
            get { return textBox1.Text; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int chosenIndex = -1;
            int numberOfItemsInEnum = Video.MovieSection.GetNames(typeof(Video.MovieSection)).Length;

            for (int i = 0; i < numberOfItemsInEnum; i++)
            {
                if ((groupBox1.Controls[i] as RadioButton).Checked)
                {
                    chosenIndex = i;
                    break;
                }
            }

            if (chosenIndex > -1)
            {
                _chosenSection = (Video.MovieSection)chosenIndex;
                _chosenName = textBox1.Text;
            }

            Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(sender, e);
        }
    }
}
