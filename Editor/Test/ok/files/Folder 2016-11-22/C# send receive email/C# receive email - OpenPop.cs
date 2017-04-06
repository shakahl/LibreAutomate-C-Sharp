 This macro uses OpenPop.NET library. Download it from https://sourceforge.net/projects/hpop/ and extract OpenPop.dll to QM folder.

out

 get password from registry HKEY_CURRENT_USER\SOFTWARE\GinDi\QM2\User\p
str password; if(!rget(password "gmail" "\p")) end "failed"

 download messages from gmail account Inbox folder
ARRAY(str) a=CsFunc("" "pop.gmail.com" 995 1 "qmgindi" password)

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
using OpenPop;
using OpenPop.Pop3;
using OpenPop.Mime;

public class Email
{
	public static string[] Receive(string server, int port, bool useSsl, string user, string password)
	{
		var a=new List<string>();
		foreach(var m in FetchAllMessages(server, port, useSsl, user, password)) {
			//Out(m.Headers.From);
			a.Add(System.Text.Encoding.UTF8.GetString(m.RawMessage));
		}
		return a.ToArray();
	}
	
	static void Out(object s) { Console.WriteLine(s); }
	
/// <summary>
/// Example showing:
///  - how to fetch all messages from a POP3 server
/// </summary>
/// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
/// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
/// <param name="useSsl">Whether or not to use SSL to connect to server</param>
/// <param name="username">Username of the user on the server</param>
/// <param name="password">Password of the user on the server</param>
/// <returns>All Messages on the POP3 server</returns>
public static List<Message> FetchAllMessages(string hostname, int port, bool useSsl, string username, string password)
{
    // The client disconnects from the server when being disposed
    using(Pop3Client client = new Pop3Client())
    {
        // Connect to the server
        client.Connect(hostname, port, useSsl);

        // Authenticate ourselves towards the server
        client.Authenticate(username, password);

        // Get the number of messages in the inbox
        int messageCount = client.GetMessageCount();

        // We want to download all messages
        List<Message> allMessages = new List<Message>(messageCount);

        // Messages are numbered in the interval: [1, messageCount]
        // Ergo: message numbers are 1-based.
        // Most servers give the latest message the highest number
        for (int i = messageCount; i > 0; i--)
        {
            allMessages.Add(client.GetMessage(i));
        }

        // Now return the fetched messages
        return allMessages;
    }
}


}
