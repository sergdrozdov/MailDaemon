namespace MailDaemon.WinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void mainMenu_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
