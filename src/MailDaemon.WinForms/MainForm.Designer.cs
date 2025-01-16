namespace MailDaemon.WinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainMenuStrip = new MenuStrip();
            mainMenu_Program = new ToolStripMenuItem();
            mainMenu_Exit = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            listRecipients = new ListView();
            clmnName = new ColumnHeader();
            clmnEmail = new ColumnHeader();
            clmnSubject = new ColumnHeader();
            clmnTemplate = new ColumnHeader();
            clmnReplace = new ColumnHeader();
            clmnSkip = new ColumnHeader();
            lblHeaderProfileName = new Label();
            lblProfileName = new Label();
            mainMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { mainMenu_Program, helpToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(1194, 24);
            mainMenuStrip.TabIndex = 0;
            mainMenuStrip.Text = "Main menu";
            // 
            // mainMenu_Program
            // 
            mainMenu_Program.DropDownItems.AddRange(new ToolStripItem[] { mainMenu_Exit });
            mainMenu_Program.Name = "mainMenu_Program";
            mainMenu_Program.Size = new Size(65, 20);
            mainMenu_Program.Text = "&Program";
            // 
            // mainMenu_Exit
            // 
            mainMenu_Exit.Name = "mainMenu_Exit";
            mainMenu_Exit.Size = new Size(93, 22);
            mainMenu_Exit.Text = "E&xit";
            mainMenu_Exit.Click += mainMenu_Exit_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(107, 22);
            aboutToolStripMenuItem.Text = "&About";
            // 
            // listRecipients
            // 
            listRecipients.Columns.AddRange(new ColumnHeader[] { clmnName, clmnEmail, clmnSubject, clmnTemplate, clmnReplace, clmnSkip });
            listRecipients.FullRowSelect = true;
            listRecipients.GridLines = true;
            listRecipients.Location = new Point(12, 103);
            listRecipients.Name = "listRecipients";
            listRecipients.ShowItemToolTips = true;
            listRecipients.Size = new Size(1170, 609);
            listRecipients.TabIndex = 1;
            listRecipients.UseCompatibleStateImageBehavior = false;
            listRecipients.View = View.Details;
            // 
            // clmnName
            // 
            clmnName.Text = "Name";
            clmnName.Width = 100;
            // 
            // clmnEmail
            // 
            clmnEmail.Text = "Email";
            clmnEmail.Width = 250;
            // 
            // clmnSubject
            // 
            clmnSubject.Text = "Subject";
            clmnSubject.Width = 300;
            // 
            // clmnTemplate
            // 
            clmnTemplate.Text = "Template";
            clmnTemplate.Width = 150;
            // 
            // clmnReplace
            // 
            clmnReplace.Text = "Replace";
            clmnReplace.Width = 300;
            // 
            // clmnSkip
            // 
            clmnSkip.Text = "Skip";
            clmnSkip.Width = 35;
            // 
            // lblHeaderProfileName
            // 
            lblHeaderProfileName.AutoSize = true;
            lblHeaderProfileName.Location = new Point(12, 52);
            lblHeaderProfileName.Name = "lblHeaderProfileName";
            lblHeaderProfileName.Size = new Size(41, 15);
            lblHeaderProfileName.TabIndex = 2;
            lblHeaderProfileName.Text = "Profile";
            // 
            // lblProfileName
            // 
            lblProfileName.AutoSize = true;
            lblProfileName.Location = new Point(71, 52);
            lblProfileName.Name = "lblProfileName";
            lblProfileName.Size = new Size(49, 15);
            lblProfileName.TabIndex = 3;
            lblProfileName.Text = "[profile]";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1194, 724);
            Controls.Add(lblProfileName);
            Controls.Add(lblHeaderProfileName);
            Controls.Add(listRecipients);
            Controls.Add(mainMenuStrip);
            MainMenuStrip = mainMenuStrip;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mail Daemon";
            mainMenuStrip.ResumeLayout(false);
            mainMenuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem mainMenu_Program;
        private ToolStripMenuItem mainMenu_Exit;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ListView listRecipients;
        private ColumnHeader clmnName;
        private ColumnHeader clmnEmail;
        private ColumnHeader clmnSubject;
        private ColumnHeader clmnTemplate;
        private Label lblHeaderProfileName;
        private Label lblProfileName;
        private ColumnHeader clmnSkip;
        private ColumnHeader clmnReplace;
    }
}
