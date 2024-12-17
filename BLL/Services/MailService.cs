using Domain.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;

namespace BLL.Services {
  public class MailService(IOptions<MailSettings> mailSettings) {
    public async Task<bool> SendMail(MailData mailData) {
      try {
        //MimeMessage - a class from Mimekit
        MimeMessage email_Message = new();
        MailboxAddress email_From = new(mailSettings.Value.Name, "info@resultats.be");
        email_Message.From.Add(email_From);
        MailboxAddress email_To = new(mailData.EmailToName, mailData.EmailToId);
        email_Message.To.Add(email_To);
        email_Message.Subject = mailData.EmailSubject;
        BodyBuilder emailBodyBuilder = new() {
          HtmlBody = mailData.EmailHTMLBody,
          TextBody = mailData.EmailTextBody
        };
        email_Message.Body = emailBodyBuilder.ToMessageBody();
        //this is the SmtpClient class from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
        SmtpClient MailClient = new();
        await MailClient.ConnectAsync(mailSettings.Value.Host, mailSettings.Value.Port, mailSettings.Value.UseSSL).ConfigureAwait(false);
        await MailClient.AuthenticateAsync(mailSettings.Value.EmailId, mailSettings.Value.Password).ConfigureAwait(false);
        Console.WriteLine(email_Message);
        await MailClient.SendAsync(email_Message).ConfigureAwait(false);
        await MailClient.DisconnectAsync(true).ConfigureAwait(false);
        MailClient.Dispose();
        return true;
      } catch (Exception ex) {
        Console.WriteLine(ex);
        // Exception Details
        return false;
      }
    }

    public async Task<bool> SendValidation(string email, int userId, string activationCode) {
      MailData mailData = new() {
        EmailToName = email,
        EmailToId = email,
        EmailSubject = "Activation du compte resultats.be",
        EmailHTMLBody = "<a href='http://localhost:4200/activation/?ActivationCode=" + WebUtility.UrlEncode(activationCode) + "&UserId=" + userId + "'>Cliquez pour activer votre compte résultats.be</a>",
        //EmailTextBody = "<a href='http://localhost:4200/activation/" + activationCode+"'>Cliquez pour activer votre compte</a>"
      };
      return await SendMail(mailData);
    }

    public async Task<bool> SendResetPassword(string email, int userId, string resetPasswordCode) {
      MailData mailData = new() {
        EmailToName = email,
        EmailToId = email,
        EmailSubject = "Récupération du mot de passe resultats.be",
        EmailHTMLBody = "<a href='http://localhost:4200/resetPassword/?ResetPasswordCode=" + WebUtility.UrlEncode(resetPasswordCode) + "&UserId=" + userId + "'>Cliquez pour réinitialiser votre mot de passe résultats.be</a>",
        //EmailTextBody = "<a href='http://localhost:4200/activation/" + activationCode+"'>Cliquez pour activer votre compte</a>"
      };
      return await SendMail(mailData);
    }
  }
}
