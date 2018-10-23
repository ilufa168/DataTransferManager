namespace IlufaDataTransfer
{
    partial class MainWindow
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
            this.B_Settings = new System.Windows.Forms.Button();
            this.B_Start_Transfer_To_Hq = new System.Windows.Forms.Button();
            this.DTP_From_Date = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Lbl_Location = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Rtb_Activity_Log = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Settings
            // 
            this.B_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.B_Settings.Location = new System.Drawing.Point(34, 464);
            this.B_Settings.Name = "B_Settings";
            this.B_Settings.Size = new System.Drawing.Size(78, 25);
            this.B_Settings.TabIndex = 0;
            this.B_Settings.Text = "Settings";
            this.B_Settings.UseVisualStyleBackColor = true;
            this.B_Settings.Click += new System.EventHandler(this.B_Settings_Click);
            // 
            // B_Start_Transfer_To_Hq
            // 
            this.B_Start_Transfer_To_Hq.ForeColor = System.Drawing.Color.DarkGreen;
            this.B_Start_Transfer_To_Hq.Location = new System.Drawing.Point(254, 56);
            this.B_Start_Transfer_To_Hq.Name = "B_Start_Transfer_To_Hq";
            this.B_Start_Transfer_To_Hq.Size = new System.Drawing.Size(85, 26);
            this.B_Start_Transfer_To_Hq.TabIndex = 1;
            this.B_Start_Transfer_To_Hq.Text = "Send";
            this.B_Start_Transfer_To_Hq.UseVisualStyleBackColor = true;
            this.B_Start_Transfer_To_Hq.Click += new System.EventHandler(this.B_Start_Transfer_To_Hq_Click);
            // 
            // DTP_From_Date
            // 
            this.DTP_From_Date.Location = new System.Drawing.Point(10, 97);
            this.DTP_From_Date.Name = "DTP_From_Date";
            this.DTP_From_Date.Size = new System.Drawing.Size(200, 22);
            this.DTP_From_Date.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.ForeColor = System.Drawing.Color.SpringGreen;
            this.groupBox1.Location = new System.Drawing.Point(34, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(387, 171);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "End of Day Processing";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.Lbl_Location);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.B_Start_Transfer_To_Hq);
            this.panel1.Controls.Add(this.DTP_From_Date);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(6, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 132);
            this.panel1.TabIndex = 5;
            // 
            // Lbl_Location
            // 
            this.Lbl_Location.AutoSize = true;
            this.Lbl_Location.Location = new System.Drawing.Point(120, 46);
            this.Lbl_Location.Name = "Lbl_Location";
            this.Lbl_Location.Size = new System.Drawing.Size(29, 16);
            this.Lbl_Location.TabIndex = 5;
            this.Lbl_Location.Text = "000";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Current Location:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Transfer Sales Beginning on:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Send Sales Updates To Jakarta";
            // 
            // groupBox2
            // 
            this.groupBox2.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.groupBox2.Location = new System.Drawing.Point(34, 218);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(387, 240);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Refresh";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // Rtb_Activity_Log
            // 
            this.Rtb_Activity_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Rtb_Activity_Log.Location = new System.Drawing.Point(445, 43);
            this.Rtb_Activity_Log.Name = "Rtb_Activity_Log";
            this.Rtb_Activity_Log.Size = new System.Drawing.Size(434, 439);
            this.Rtb_Activity_Log.TabIndex = 5;
            this.Rtb_Activity_Log.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.label3.Location = new System.Drawing.Point(445, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Activity Log";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(891, 494);
            this.Controls.Add(this.B_Settings);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Rtb_Activity_Log);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Green;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainWindow";
            this.Text = "Ilufa Data Transder";
            this.Activated += new System.EventHandler(this.MainWindow_Activated);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Settings;
        private System.Windows.Forms.Button B_Start_Transfer_To_Hq;
        private System.Windows.Forms.DateTimePicker DTP_From_Date;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox Rtb_Activity_Log;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Lbl_Location;
        private System.Windows.Forms.Label label4;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

