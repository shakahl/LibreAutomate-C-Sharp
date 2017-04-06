CsFunc("" "password" "to@example.com" "Subject" "Body")


#ret
//C# code
using System;
using System.Net;
using System.Net.Mail;

public class Email
{
public static void Send(string password, string email, string subject, string body)
{
var fromAddress = new MailAddress("qmgindi@gmail.com", "From Name");
var toAddress = new MailAddress(email);

var smtp = new SmtpClient
{
    Host = "smtp.gmail.com",
    Port = 587,
    EnableSsl = true,
    DeliveryMethod = SmtpDeliveryMethod.Network,
    UseDefaultCredentials = false,
    Credentials = new NetworkCredential(fromAddress.Address, password)
};
using (var message = new MailMessage(fromAddress, toAddress)
{
    Subject = subject,
    Body = body
})
{
    smtp.Send(message);
}
}
}
