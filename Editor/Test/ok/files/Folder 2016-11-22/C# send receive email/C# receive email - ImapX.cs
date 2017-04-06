 This macro uses ImapX .NET library. Download it from http://imapx.codeplex.com/ and extract ImapX.dll to QM folder.

out

 get password from registry HKEY_CURRENT_USER\SOFTWARE\GinDi\QM2\User\p
str password; if(!rget(password "gmail" "\p")) end "failed"

 download messages from gmail account Inbox folder
ARRAY(str) a=CsFunc("" "me@gmail.com" password "ALL" 0)
 info:
 Replace 0 to 1 to mark the messages as seen.
 Replace "ALL" to "UNSEEN" to get only unseen messages.
 All search filters: https://tools.ietf.org/html/rfc3501#section-6.4.4
 The function does not delete messages on the server. You can modify it to delete or move to another folder from Inbox. Also you can modify it to get messages from another folder etc.
 ImapX has only some basic documentation: http://imapx.codeplex.com/documentation

 parse and display messages
out a.len
int i
for i 0 a.len
	MailBee.Message m._create
	m.RawBody=a[i]
	str from(m.FromAddr) subject(m.Subject) body
	body=iif(m.BodyFormat=1 m.GetPlainFromHtml(m.BodyText) m.BodyText)
	out F"<><Z 0x80E080>{from}    {subject}</Z>[]{body}"
	


#ret
//C# code
using System;
using System.Collections.Generic;
using ImapX;

public class Email
{
	public static string[] Receive(string user, string password, string filter="ALL", bool markSeen=false)
	{
		using(var client = new ImapClient("imap.googlemail.com", true)) {
			//client.Port=993; client.UseSsl=true; //default
			if(!client.Connect()) { Out("failed to connect"); return null; }
			if(!client.Login(user, password)) { Out("failed to login"); return null; }
			var folder = client.Folders.Inbox;
			List<string> a = new List<string>();
			foreach(var m in folder.Search(filter, ImapX.Enums.MessageFetchMode.Tiny)) {
				//Out(m.From);
				if(markSeen) m.Seen = true;
				//a.Add(m.ToEml());
				a.Add(m.DownloadRawMessage());
			}
			return a.ToArray();
		}
	}
	
	static void Out(object s) { Console.WriteLine(s); }
}
