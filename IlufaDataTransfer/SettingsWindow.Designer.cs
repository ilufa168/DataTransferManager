namespace IlufaDataTransfer
{
    partial class SettingsWindow
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
            this.label1 = new System.Windows.Forms.Label();
            this.CB_Location = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Txt_Oracle_IP = new System.Windows.Forms.TextBox();
            this.Txt_Remote_IP = new System.Windows.Forms.TextBox();
            this.Txt_VPN_Name = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ButtonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Location:";
            // 
            // CB_Location
            // 
            this.CB_Location.FormattingEnabled = true;
            this.CB_Location.Items.AddRange(new object[] {
            "A01",
            "A02",
            "A03",
            "A04",
            "A05",
            "A06",
            "A07",
            "A08",
            "A09",
            "A99"});
            this.CB_Location.Location = new System.Drawing.Point(179, 28);
            this.CB_Location.Name = "CB_Location";
            this.CB_Location.Size = new System.Drawing.Size(121, 24);
            this.CB_Location.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "VPN Connection Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Remote Oracle IP:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "Oracle IP Address:";
            // 
            // Txt_Oracle_IP
            // 
            this.Txt_Oracle_IP.Location = new System.Drawing.Point(179, 71);
            this.Txt_Oracle_IP.Name = "Txt_Oracle_IP";
            this.Txt_Oracle_IP.Size = new System.Drawing.Size(234, 22);
            this.Txt_Oracle_IP.TabIndex = 6;
            // 
            // Txt_Remote_IP
            // 
            this.Txt_Remote_IP.Enabled = false;
            this.Txt_Remote_IP.Location = new System.Drawing.Point(179, 108);
            this.Txt_Remote_IP.Name = "Txt_Remote_IP";
            this.Txt_Remote_IP.Size = new System.Drawing.Size(234, 22);
            this.Txt_Remote_IP.TabIndex = 7;
            this.Txt_Remote_IP.Text = "192.168.1.1";
            // 
            // Txt_VPN_Name
            // 
            this.Txt_VPN_Name.Location = new System.Drawing.Point(179, 146);
            this.Txt_VPN_Name.Name = "Txt_VPN_Name";
            this.Txt_VPN_Name.Size = new System.Drawing.Size(234, 22);
            this.Txt_VPN_Name.TabIndex = 8;
            this.Txt_VPN_Name.Text = "IL1";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(33, 189);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(380, 209);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Notification Setup";
            // 
            // ButtonSave
            // 
            this.ButtonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSave.Location = new System.Drawing.Point(338, 404);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(75, 23);
            this.ButtonSave.TabIndex = 10;
            this.ButtonSave.Text = "Save";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LemonChiffon;
            this.ClientSize = new System.Drawing.Size(453, 442);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Txt_VPN_Name);
            this.Controls.Add(this.Txt_Remote_IP);
            this.Controls.Add(this.Txt_Oracle_IP);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CB_Location);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.DarkBlue;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SettingsWindow";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CB_Location;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Txt_Oracle_IP;
        private System.Windows.Forms.TextBox Txt_Remote_IP;
        private System.Windows.Forms.TextBox Txt_VPN_Name;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ButtonSave;
    }
}