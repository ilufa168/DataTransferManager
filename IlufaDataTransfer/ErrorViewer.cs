using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;

namespace IlufaDataTransfer
{
    public partial class ErrorViewer : Form
    {
        List<String> error_list;

        public ErrorViewer()
        {
            InitializeComponent();
        }

        public ErrorViewer(List<string> errors)
        {
            this.error_list = errors;
            InitializeComponent();
            DisplayErrors();
        }

        private void DisplayErrors()
        {
            foreach(string err in error_list)
            {
                rtbErrors.Text += err + Environment.NewLine;
            }
            rtbErrors.Refresh();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MailMessage mail = new MailMessage("ilufatransfer@gmail.com", "ilufatransfer@gmail.com");
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("ilufatransfer@gmail.com", "1681ndonesia");
            mail.Subject = "Errors ba ba ba";
            mail.BodyEncoding = UTF8Encoding.UTF8;
            mail.Body = this.rtbErrors.Text;
            client.Send(mail);
            MessageBox.Show("Mail sent.");
        }
    }
}
