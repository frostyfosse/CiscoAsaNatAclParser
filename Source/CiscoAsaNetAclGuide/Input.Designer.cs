namespace CiscoAsaNetAclParser
{
    partial class Input
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.logContent = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.clearButton = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.parseButton = new System.Windows.Forms.Button();
            this.outputPath = new System.Windows.Forms.Label();
            this.objectNetworkFilenameLabel2 = new System.Windows.Forms.Label();
            this.objectNetworkFilenameLabel1 = new System.Windows.Forms.Label();
            this.accessListFilenameLabel1 = new System.Windows.Forms.Label();
            this.accessListFilenameLabel2 = new System.Windows.Forms.Label();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogFileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.browseOutputFolderMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openOutputDirectoryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.generateOutputMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAppMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.clearWindowMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // logContent
            // 
            this.logContent.Location = new System.Drawing.Point(13, 58);
            this.logContent.Name = "logContent";
            this.logContent.Size = new System.Drawing.Size(489, 394);
            this.logContent.TabIndex = 0;
            this.logContent.Text = "";
            this.logContent.TextChanged += new System.EventHandler(this.DataInWindowChangedEvent);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Log Data Window";
            // 
            // clearButton
            // 
            this.clearButton.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::CiscoAsaNetAclParser.Properties.Settings.Default, "EnableButtons", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.clearButton.Enabled = global::CiscoAsaNetAclParser.Properties.Settings.Default.EnableButtons;
            this.clearButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearButton.Location = new System.Drawing.Point(13, 458);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(122, 39);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearDataInWindow);
            // 
            // status
            // 
            this.status.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.status.Location = new System.Drawing.Point(298, 13);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(204, 23);
            this.status.TabIndex = 3;
            this.status.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // browseButton
            // 
            this.browseButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseButton.Location = new System.Drawing.Point(13, 516);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(122, 39);
            this.browseButton.TabIndex = 4;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseFoldersForOutputDirectory);
            // 
            // parseButton
            // 
            this.parseButton.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::CiscoAsaNetAclParser.Properties.Settings.Default, "EnableButtons", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.parseButton.Enabled = global::CiscoAsaNetAclParser.Properties.Settings.Default.EnableButtons;
            this.parseButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.parseButton.Location = new System.Drawing.Point(376, 458);
            this.parseButton.Name = "parseButton";
            this.parseButton.Size = new System.Drawing.Size(122, 39);
            this.parseButton.TabIndex = 2;
            this.parseButton.Text = "Generate";
            this.parseButton.UseVisualStyleBackColor = true;
            this.parseButton.Click += new System.EventHandler(this.GenerateOutputFiles);
            // 
            // outputPath
            // 
            this.outputPath.AutoEllipsis = true;
            this.outputPath.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputPath.Location = new System.Drawing.Point(141, 521);
            this.outputPath.Name = "outputPath";
            this.outputPath.Size = new System.Drawing.Size(361, 28);
            this.outputPath.TabIndex = 7;
            this.outputPath.Text = "My path";
            this.outputPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // objectNetworkFilenameLabel2
            // 
            this.objectNetworkFilenameLabel2.AutoSize = true;
            this.objectNetworkFilenameLabel2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.objectNetworkFilenameLabel2.Location = new System.Drawing.Point(208, 559);
            this.objectNetworkFilenameLabel2.Name = "objectNetworkFilenameLabel2";
            this.objectNetworkFilenameLabel2.Size = new System.Drawing.Size(77, 16);
            this.objectNetworkFilenameLabel2.TabIndex = 8;
            this.objectNetworkFilenameLabel2.Text = "My filename";
            this.objectNetworkFilenameLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // objectNetworkFilenameLabel1
            // 
            this.objectNetworkFilenameLabel1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.objectNetworkFilenameLabel1.Location = new System.Drawing.Point(49, 559);
            this.objectNetworkFilenameLabel1.Name = "objectNetworkFilenameLabel1";
            this.objectNetworkFilenameLabel1.Size = new System.Drawing.Size(153, 16);
            this.objectNetworkFilenameLabel1.TabIndex = 9;
            this.objectNetworkFilenameLabel1.Text = "Object Network Filename";
            this.objectNetworkFilenameLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // accessListFilenameLabel1
            // 
            this.accessListFilenameLabel1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accessListFilenameLabel1.Location = new System.Drawing.Point(49, 584);
            this.accessListFilenameLabel1.Name = "accessListFilenameLabel1";
            this.accessListFilenameLabel1.Size = new System.Drawing.Size(153, 16);
            this.accessListFilenameLabel1.TabIndex = 11;
            this.accessListFilenameLabel1.Text = "Access List Filename";
            this.accessListFilenameLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // accessListFilenameLabel2
            // 
            this.accessListFilenameLabel2.AutoSize = true;
            this.accessListFilenameLabel2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accessListFilenameLabel2.Location = new System.Drawing.Point(208, 584);
            this.accessListFilenameLabel2.Name = "accessListFilenameLabel2";
            this.accessListFilenameLabel2.Size = new System.Drawing.Size(77, 16);
            this.accessListFilenameLabel2.TabIndex = 10;
            this.accessListFilenameLabel2.Text = "My filename";
            this.accessListFilenameLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.AccessibleName = "";
            this.mainMenuStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.mainMenuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.mainMenuStrip.Size = new System.Drawing.Size(523, 25);
            this.mainMenuStrip.TabIndex = 12;
            this.mainMenuStrip.Text = "formMenu";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateOutputMenu,
            this.clearWindowMenu,
            this.browseOutputFolderMenu,
            this.openOutputDirectoryMenu,
            this.openLogFileMenu,
            this.closeAppMenu});
            this.menuToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(53, 21);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // openLogFileMenu
            // 
            this.openLogFileMenu.Name = "openLogFileMenu";
            this.openLogFileMenu.Size = new System.Drawing.Size(271, 22);
            this.openLogFileMenu.Text = "Open Log File";
            this.openLogFileMenu.Click += new System.EventHandler(this.OpenLogFile);
            // 
            // browseOutputFolderMenu
            // 
            this.browseOutputFolderMenu.Name = "browseOutputFolderMenu";
            this.browseOutputFolderMenu.Size = new System.Drawing.Size(271, 22);
            this.browseOutputFolderMenu.Text = "Browse Output Directory";
            this.browseOutputFolderMenu.Click += new System.EventHandler(this.BrowseFoldersForOutputDirectory);
            // 
            // openOutputDirectoryMenu
            // 
            this.openOutputDirectoryMenu.Name = "openOutputDirectoryMenu";
            this.openOutputDirectoryMenu.Size = new System.Drawing.Size(271, 22);
            this.openOutputDirectoryMenu.Text = "Open Output Directory";
            this.openOutputDirectoryMenu.Click += new System.EventHandler(this.OpenOutputDirectory);
            // 
            // generateOutputMenu
            // 
            this.generateOutputMenu.Enabled = global::CiscoAsaNetAclParser.Properties.Settings.Default.EnableButtons;
            this.generateOutputMenu.Name = "generateOutputMenu";
            this.generateOutputMenu.Size = new System.Drawing.Size(271, 22);
            this.generateOutputMenu.Text = "Generate New Configuration Files";
            this.generateOutputMenu.Click += new System.EventHandler(this.GenerateOutputFiles);
            // 
            // closeAppMenu
            // 
            this.closeAppMenu.Name = "closeAppMenu";
            this.closeAppMenu.Size = new System.Drawing.Size(271, 22);
            this.closeAppMenu.Text = "Close";
            this.closeAppMenu.Click += new System.EventHandler(this.CloseApp);
            // 
            // clearWindowMenu
            // 
            this.clearWindowMenu.Enabled = global::CiscoAsaNetAclParser.Properties.Settings.Default.EnableButtons;
            this.clearWindowMenu.Name = "clearWindowMenu";
            this.clearWindowMenu.Size = new System.Drawing.Size(271, 22);
            this.clearWindowMenu.Text = "Clear Data in Window";
            this.clearWindowMenu.Click += new System.EventHandler(this.ClearDataInWindow);
            // 
            // Input
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 615);
            this.Controls.Add(this.accessListFilenameLabel1);
            this.Controls.Add(this.accessListFilenameLabel2);
            this.Controls.Add(this.objectNetworkFilenameLabel1);
            this.Controls.Add(this.objectNetworkFilenameLabel2);
            this.Controls.Add(this.outputPath);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.status);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.parseButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logContent);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "Input";
            this.Text = "Cisco Asa Parser";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox logContent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button parseButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label outputPath;
        private System.Windows.Forms.Label objectNetworkFilenameLabel2;
        private System.Windows.Forms.Label objectNetworkFilenameLabel1;
        private System.Windows.Forms.Label accessListFilenameLabel1;
        private System.Windows.Forms.Label accessListFilenameLabel2;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogFileMenu;
        private System.Windows.Forms.ToolStripMenuItem openOutputDirectoryMenu;
        private System.Windows.Forms.ToolStripMenuItem browseOutputFolderMenu;
        private System.Windows.Forms.ToolStripMenuItem closeAppMenu;
        private System.Windows.Forms.ToolStripMenuItem generateOutputMenu;
        private System.Windows.Forms.ToolStripMenuItem clearWindowMenu;
    }
}

