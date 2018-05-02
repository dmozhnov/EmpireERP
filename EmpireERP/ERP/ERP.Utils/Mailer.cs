using System.Net;

namespace ERP.Utils
{
	/// <summary>
	/// ����� ��� �������� ����������� �����
	/// </summary>
	public class Mailer
	{
        private string senderEmail;
        private string senderName;
        private string smtpServer;
        private string smtpLogin;
        private string smtpPassword;

        public Mailer(string senderEmail, string senderName, string smtpServer, string smtpLogin, string smtpPassword)
        {
            this.senderEmail = senderEmail;
            this.senderName = senderName;
            this.smtpServer = smtpServer;
            this.smtpLogin = smtpLogin;
            this.smtpPassword = smtpPassword;
        }
                
        /// <summary>
		/// ��������� �����
		/// </summary>
		/// <param name="to">����������</param>
		/// <param name="subject">����</param>
		/// <param name="body">���� ������</param>
		public void SendMail(string to, string subject, string body)
		{
            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();

            email.IsBodyHtml = true;
            email.BodyEncoding = System.Text.Encoding.UTF8;
            email.SubjectEncoding = System.Text.Encoding.GetEncoding("windows-1251");

            email.From = new System.Net.Mail.MailAddress(senderEmail, senderName, System.Text.Encoding.GetEncoding("windows-1251"));
            email.To.Add(new System.Net.Mail.MailAddress(to));
            email.Subject = subject;
            email.Body = body;
            
            System.Net.Mail.SmtpClient server = new System.Net.Mail.SmtpClient();
            server.Host = smtpServer;
            server.Credentials = new NetworkCredential(smtpLogin, smtpPassword);            

            server.Send(email);            
		}
        		
	}
}
