/// Use NuGet package <+nuget>MailKit<>. Links: <link https://github.com/jstedfast/MailKit>info and examples<>, <link https://github.com/jstedfast/MailKit/blob/master/FAQ.md>FAQ<>.
/// 
/// This recipe shows how to connect to Gmail and send/receive. See also Gmail setup in recipe <+recipe>send email<>.

/*/ nuget -\MailKit; /*/

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MailKit;
using MailKit.Net.Imap;

/// Send message.

var message = new MimeMessage();
message.From.Add(new MailboxAddress("Cat", "from@email.com"));
message.To.Add(new MailboxAddress("Mouse", "to@email.com"));
message.Subject = "MailKit";
message.Body = new TextPart("plain") { Text = @"message text" };

using (var client = new SmtpClient()) {
	client.CheckCertificateRevocation = false;
	client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
	client.Authenticate("username", "password");
	client.Send(message);
	client.Disconnect(true);
}

/// Receive messages.

using (var client = new ImapClient()) {
	client.CheckCertificateRevocation = false;
	client.Connect("imap.googlemail.com", 993, true);
	client.Authenticate("username", "password");

	var inbox = client.Inbox;
	inbox.Open(FolderAccess.ReadOnly);

	print.it($"Count {inbox.Count}, recent {inbox.Recent}");

	for (int i = 0, n = Math.Min(inbox.Count, 5); i < n; i++) {
		var m = inbox.GetMessage(i);
		print.it(m.Subject);
	}

	client.Disconnect(true);
}
