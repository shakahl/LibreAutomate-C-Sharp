function'MailBee.Messages [flags] [nbodylines] [$account] ;;flags: 1 headers, 2 delete, 0x100 dialog, 0x200 events, 0x80000 log.

 Downloads email messages from POP3 server to MailBee.Messages collection.
 Error if fails.

 flags:
	 1 - get only headers and optionally max nbodylines lines of body (including attachments).
	 2 - delete messages from server. If this flag is not set, mail remains on server, but, if you call Connect before calling this function, you can selectively delete messages after calling this function.
	 The following flags are used by Pop3Mail.Connect, and ignored if already connected:
	 0x100 - while receiving email, show progress dialog.
	 0x200 - while receiving email, process Windows messages and COM events.
	    Use this flag, for example, when calling this function in a thread with a dialog. Without it the dialog hangs temporarily.
	    Also, the MailBee.POP3 object (member variable p) will fire COM events itself.
	    Flag 0x100 also does the same.
	 debugging flags:
	    0x80000 - log to file "$my qm$\pop3_log.txt". Logs entire session (not only while connecting).
 nbodylines - see flag 1.
 account - same as with <help>ReceiveMail</help>. Ignored if already connected.

 REMARKS
 Can be already connected or not. If not connected, connects and disconnects.

 See also: <ReceiveMail>


str s s1 s2
int i conn=p.Connected
if(!conn) Connect(flags account); err end _error

 p.CodepageMode=1 ;;workaround for mailbee bug: incorrectly saves binary attachment if charset specified, like Content-Type: image/jpeg; charset="UTF-8"... However then incorrectly interprets UTF8 text.

MailBee.Messages mc
if(flags&1) mc=p.RetrieveHeaders(nbodylines)
else mc=p.RetrieveMessages()
if(mc and flags&2) for(i 1 mc.Count+1) p.DeleteMessage(i)

if(!mc) s.format("cannot retrieve messages: %s (0x%X)[][9]Server response: %s" s1.from(p.ErrDesc) p.ErrCode s2.from(p.ServerResponse))
if(!conn) Disconnect
if(mc) ret mc
end s
