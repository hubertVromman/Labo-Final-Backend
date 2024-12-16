using Domain.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace BLL.Services {
  public class MailService(IOptions<MailSettings> mailSettings) {
    public bool SendMail(MailData mailData) {
      try {
        //MimeMessage - a class from Mimekit
        MimeMessage email_Message = new();
        MailboxAddress email_From = new(mailSettings.Value.Name, "info@resultats.be");
        email_Message.From.Add(email_From);
        MailboxAddress email_To = new(mailData.EmailToName, mailData.EmailToId);
        email_Message.To.Add(email_To);
        email_Message.Subject = mailData.EmailSubject;
        BodyBuilder emailBodyBuilder = new() {
          TextBody = mailData.EmailBody
        };
        email_Message.Body = emailBodyBuilder.ToMessageBody();
        //this is the SmtpClient class from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
        SmtpClient MailClient = new ();
        MailClient.Connect(mailSettings.Value.Host, mailSettings.Value.Port, mailSettings.Value.UseSSL);
        MailClient.Authenticate(mailSettings.Value.EmailId, mailSettings.Value.Password);
        MailClient.Send(email_Message);
        MailClient.Disconnect(true);
        MailClient.Dispose();
        return true;
      } catch (Exception ex) {
                Console.WriteLine(ex);
        // Exception Details
        return false;
      }
    }
  }
} 

//{
//  "emailToId": "hubert.vromman@gmail.com",
//  "emailToName": "hub",
//  "emailSubject": "test",
//  "emailBody": "coucou"
//  }
