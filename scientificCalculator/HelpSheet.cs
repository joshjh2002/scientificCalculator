using System;
using System.Windows.Forms;

namespace scientificCalculator
{
    public partial class HelpSheet : Form
    {
        public HelpSheet(string title, string helpText)
        {
            this.Text = title + " Help Sheet";
            RichTextBox helpTextTextbox = new RichTextBox() { Text = helpText, Dock = DockStyle.Fill, ReadOnly = true };
            this.Width = 400;
            this.Height = 600;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Controls.Add(helpTextTextbox);
        }
    }
}
