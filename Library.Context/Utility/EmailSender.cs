using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;

namespace Library.Context.Utility
{
    public class EmailSender
    {
        public static string Send(string to, string cc, string subject, string body, string[] attachments)
        {
            EmailSender sender = new EmailSender();
            MailMessage msg = sender.PrepareMessage(to, cc, subject, body, attachments);
            return sender.Send(msg);
        }

        public static string SendAsync(string to, string cc, string subject, string body)
        {
            EmailSender sender = new EmailSender();
            MailMessage msg = sender.PrepareMessage(to, cc, subject, body, null);
            try
            {
                Thread emailThread = new Thread(() => sender.Send(msg));
                emailThread.Start();
                return "OK";
            }
            catch (Exception )
            {
                return "ERROR";
            }
        }

        public static string SendAsync(string to, string cc, string subject, string body, string[] attachments)
        {
            EmailSender sender = new EmailSender();
            MailMessage msg = sender.PrepareMessage(to, cc, subject, body, attachments);
            try
            {
                Thread emailThread = new Thread(() => sender.Send(msg));
                emailThread.Start();
                return "OK";
            }
            catch (Exception )
            {
                return "ERROR";
            }

        }
        private string Send(MailMessage msg)
        {
            SmtpClient smtp = null;
            try
            {
                smtp = GetSmtpClient();
                smtp.Send(msg);
                return "OK";
            }
            catch (Exception )
            {
                return "ERROR";
            }
            finally
            {
                if (smtp != null)
                {
                    smtp.Dispose();
                }
            }
        }

        private MailMessage PrepareMessage(string to, string cc, string subject, string body, string[] attachments)
        {
            MailMessage msg = new MailMessage();

            try
            {
                msg.From = new MailAddress(new AppSettingsReader().GetValue("EMAIL_FROM_ADDRESS", typeof(string)).ToString());
            }
            catch
            {
                throw new Exception("Incorrect configuration settings.");
            }

            if (to != "")
            {
                var splittedTo = to.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splittedTo.Length; i++)
                {
                    try
                    {
                        MailAddress ccAddress = new MailAddress(splittedTo[i]);
                        msg.To.Add(ccAddress);
                    }
                    catch (Exception )
                    {
                        continue;
                    }
                }

            }
            else
            {
                throw new Exception("No email address in to field");
            }

            if (msg.To.Count == 0)
            {
                throw new Exception("No valid email address in to field");
            }

            if (cc != "")
            {
                var splittedCc = cc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splittedCc.Length; i++)
                {
                    try
                    {
                        MailAddress ccAddress = new MailAddress(splittedCc[i]);
                        msg.CC.Add(ccAddress);
                    }
                    catch (Exception )
                    {
                        continue;
                    }
                }

            }

            if (attachments != null)
            {
                for (int i = 0; i < attachments.Length; i++)
                {
                    Attachment attached = new Attachment(attachments[i]);
                    msg.Attachments.Add(attached);
                }
            }
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, new ContentType("text/html"));
            msg.AlternateViews.Add(htmlView);
            return msg;
        }

        private SmtpClient GetSmtpClient()
        {
            SmtpClient smtp = new SmtpClient();
            try
            {
                string host = new AppSettingsReader().GetValue("EMAIL_SMTP_SERVER", typeof(string)).ToString();
                int port = Convert.ToInt16(new AppSettingsReader().GetValue("EMAIL_SMTP_PORT", typeof(string)).ToString());
                string username = new AppSettingsReader().GetValue("EMAIL_USERNAME", typeof(string)).ToString();
                string password = new AppSettingsReader().GetValue("EMAIL_PASSWORD", typeof(string)).ToString();
                bool isSsl = Convert.ToBoolean(new AppSettingsReader().GetValue("EMAIL_SECURED", typeof(string)).ToString());

                smtp = new SmtpClient(host, port);
                NetworkCredential cred = new NetworkCredential(username, password);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = cred;
                smtp.EnableSsl = isSsl;
            }
            catch
            {
                throw new Exception("Incorrect configuration settings.");
            }
            return smtp;
        }
    }
}
