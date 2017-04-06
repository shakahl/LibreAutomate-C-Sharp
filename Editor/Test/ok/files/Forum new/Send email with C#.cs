str CSharpCode=
 using System;
 using System.Net;
 using System.Net.Mail;
 
 public class Email
 {
 static public void Send(string password, string email, string subject, string body)
 {
 var fromAddress = new MailAddress("xxxxxxxxxxxxxxxxxx@outlook.com", "From Name");
 var toAddress = new MailAddress(email);
 
 var smtp = new SmtpClient
 {
     Host = "smtp-mail.outlook.com",
     Port = 25,
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
CsFunc(CSharpCode "password" "xxxxxxxxxxxxxxxxxx@foxmail.com" "Subject" "Body")
