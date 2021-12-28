using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using VChatCore.common;
using VChatCore.Dto;
using VChatCore.Model;

namespace VChatCore.Service
{
    public class MailService
    {
        public string GetEmailBody(User user)
        {
            string url = Gobal.DomainName + "api/user/confirmMail/" + user.UserName;
            
            return string.Format(
                @"<div style='text-align:center;'>
                    <h1>Welcome to our Website</h1>
                       <h3>Click For Confirm</h3>
                        <form method='post' action={0} style='display:inline;'>
                    <button type='submit' style='display:block; text-align:center;'>Confirm Mail</button>
                    </form>
                    </div>",url

                );
        }
        public async Task<string> SendMail(MailClass mailClass)
        {
            try{
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(mailClass.EmailFrom);
                    mailClass.ToEmailIds.ForEach(x =>
                    {
                        mail.To.Add(x);
                    });
                    mail.Subject = mailClass.Subject;
                    mail.Body = mailClass.Body;
                    mail.IsBodyHtml = mailClass.IsBodyHtml;
                    mailClass.Attachments.ForEach(x =>
                    {
                        mail.Attachments.Add(new Attachment(x));
                    });
                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com",587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential(mailClass.EmailFrom,mailClass.EmailPassword);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                        return "Send Mail Success";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
