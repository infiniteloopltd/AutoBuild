using System;
using System.Configuration;
using System.Net.Mail;

/// <summary>
///  email
/// </summary>
public class Email
{
    public static string Send(string recipient, string subject, string body)
    {
        string HOST = ConfigurationManager.AppSettings["MAILSERVER"];
        string SMTP_USERNAME = ConfigurationManager.AppSettings["EMAIL_USERNAME"];
        string SMTP_PASSWORD = ConfigurationManager.AppSettings["EMAIL_PASSWORD"];
        string FROM = ConfigurationManager.AppSettings["FROM_ADDRESS"];

        SmtpClient client = new SmtpClient(HOST, 25);

        // Create a network credential with your SMTP user name and password.
        client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

        // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
        // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
        client.EnableSsl = true;



        // Send the email. 
        var strError = "";
        try
        {
            var mail = new MailMessage
            {
                From = new MailAddress(FROM),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(recipient);
            client.Send(mail);

        }
        catch (Exception ex)
        {
            strError = ex.ToString();
            if (ex.InnerException != null)
            {
                strError += ex.InnerException.ToString();
            }

        }
        return strError;

    }
}