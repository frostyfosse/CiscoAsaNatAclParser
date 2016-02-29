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
            this.outputPath = new System.Windows.Forms.TextBox();
            this.parseButton = new System.Windows.Forms.Button();
            this.openLogButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logContent
            // 
            this.logContent.Location = new System.Drawing.Point(13, 44);
            this.logContent.Name = "logContent";
            this.logContent.Size = new System.Drawing.Size(489, 408);
            this.logContent.TabIndex = 0;
            this.logContent.Text = "";
            this.logContent.TextChanged += new System.EventHandler(this.logContent_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
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
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
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
            this.browseButton.Location = new System.Drawing.Point(13, 527);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(122, 28);
            this.browseButton.TabIndex = 4;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // outputPath
            // 
            this.outputPath.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.outputPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputPath.Location = new System.Drawing.Point(148, 530);
            this.outputPath.Name = "outputPath";
            this.outputPath.Size = new System.Drawing.Size(354, 22);
            this.outputPath.TabIndex = 5;
            this.outputPath.Text = "My text";
            // 
            // parseButton
            // 
            this.parseButton.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::CiscoAsaNetAclParser.Properties.Settings.Default, "EnableButtons", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.parseButton.Enabled = global::CiscoAsaNetAclParser.Properties.Settings.Default.EnableButtons;
            this.parseButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.parseButton.Location = new System.Drawing.Point(380, 458);
            this.parseButton.Name = "parseButton";
            this.parseButton.Size = new System.Drawing.Size(122, 39);
            this.parseButton.TabIndex = 2;
            this.parseButton.Text = "Generate";
            this.parseButton.UseVisualStyleBackColor = true;
            this.parseButton.Click += new System.EventHandler(this.parseButton_Click);
            // 
            // openLogButton
            // 
            this.openLogButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openLogButton.Location = new System.Drawing.Point(12, 583);
            this.openLogButton.Name = "openLogButton";
            this.openLogButton.Size = new System.Drawing.Size(122, 28);
            this.openLogButton.TabIndex = 6;
            this.openLogButton.Text = "Open Log";
            this.openLogButton.UseVisualStyleBackColor = true;
            this.openLogButton.Click += new System.EventHandler(this.openLogButton_Click);
            // 
            // Input
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 623);
            this.Controls.Add(this.openLogButton);
            this.Controls.Add(this.outputPath);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.status);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.parseButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logContent);
            this.Name = "Input";
            this.Text = "Cisco Asa Parser";
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
        private System.Windows.Forms.TextBox outputPath;
        private System.Windows.Forms.Button openLogButton;
    }
}

