using MailDaemon.Core;

namespace MailDaemon.WinForms
{
    public partial class MainForm : Form
    {
        private static SettingsInfo SettingsInfo;
        private static IMailDaemonService mailDaemonService;
        private static IMailProfileService mailProfileService;
        private static IMailMessageService mailMessageService;
        private static MailProfile mailProfile;

        public MainForm()
        {
            SettingsInfo = new SettingsInfo();
            mailDaemonService = new MailDaemonService();
            mailProfileService = new JsonMailProfileService();
            mailMessageService = new MailMessageService();

            SettingsInfo.AppDirectory = AppDomain.CurrentDomain.BaseDirectory;

            mailProfile = new MailProfile();

            SettingsInfo.MailProfile = "mailSettings_RepeatJob - 2025-01-09.json";
            SettingsInfo.Operator = new SenderInfo() { Address = "test@test.com"};
            mailProfile.MailBodyTemplateFileName = SettingsInfo.MailProfile;
            mailProfile.MailBodyTemplateFullPath = "D:\\Programs\\MailDaemon\\MailProfiles\\mailSettings_RepeatJob - 2025-01-09.json";

            InitializeComponent();

            LoadProfile();
        }

        private void LoadProfile()
        {
            mailProfile = mailProfileService.ReadProfile(mailProfile.MailBodyTemplateFullPath);
            mailProfile.MailBody = ""; // stub
            listRecipients.Items.Clear();

            foreach (var recipient in mailProfile.Recipients)
            {
                recipient.MailBody = mailProfile.MailBody; // stub
                var mailMessage = mailMessageService.GenerateMailMessage(SettingsInfo.Operator, mailProfile, recipient);

                //var lvi = new ListViewItem(new string[]
                //{
                //    recipient.Name,
                //    recipient.Address,
                //    mailMessage.Subject,
                //    Path.GetFileName(recipient.MailBodyTemplateFileName),
                //    string.Join(",", recipient.Replace.ToList()),
                //    recipient.Skip.GetValueOrDefault() ? "x" : ""
                //});
                var lvi = new ListViewItem();
                lvi.Text = recipient.Name;
                lvi.SubItems.Add(new ListViewItem.ListViewSubItem { Text = recipient.Address });
                lvi.SubItems.Add(new ListViewItem.ListViewSubItem { Text = mailMessage.Subject });
                lvi.SubItems.Add(new ListViewItem.ListViewSubItem { Text = Path.GetFileName(recipient.MailBodyTemplateFileName) });
                lvi.SubItems.Add(new ListViewItem.ListViewSubItem { Text = string.Join(",", recipient.Replace.ToList()) });
                lvi.SubItems.Add(new ListViewItem.ListViewSubItem { Text = recipient.Skip.GetValueOrDefault() ? "x" : "" });
                listRecipients.Items.Add(lvi);
            }

            // raw
            //foreach (var recipient in mailProfile.Recipients)
            //{
            //    var lvi = new ListViewItem(new string[]
            //    {
            //        recipient.Name,
            //        recipient.Address,
            //        recipient.Subject,
            //        Path.GetFileName(recipient.MailBodyTemplateFileName),
            //        string.Join(",", recipient.Replace.ToList()),
            //        recipient.Skip.GetValueOrDefault() ? "x" : ""
            //    });
            //    listRecipients.Items.Add(lvi);
            //}
        }

        private void mainMenu_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
