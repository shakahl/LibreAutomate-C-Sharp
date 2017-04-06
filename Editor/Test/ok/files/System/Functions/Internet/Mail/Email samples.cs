 EXAMPLES

 Sending single plain-text message
SendMail "test@test.com" "subject" "text"

 Sending html message with attachments, specifying bcc and more headers, friendly names
SendMail "J. Smith <js@x.com>" "subject" "$desktop$\test.htm" 1|2 "$desktop$\test1.txt[]$desktop$\test2.txt" "" "rec2@test.com, rec3@test.com" "From: somebody@x.com[]Reply-To: somebody2@x.com[]X-Mailer: My App"

 Sending all messages (.eml files) from "mail" folder
SendMail "" "" "" 0 "" "" "" "" "$desktop$\mail"

 Sending multiple messages without reconnecting. This sample also shows progress.
SmtpMail mail
mail.AddMessage("a@test.com" "subject1" "text1")
mail.AddMessage("b@test.com" "subject2" "text2")
mail.AddMessage("c@test.com" "subject3" "text3")
mail.Send(0x100)

 To retrieve messages, can be used function ReceiveMail, or class Pop3Mail, as in the following examples.

 Retrieving all messages.
 Downloads messages from default email account. Shows download progress.
 For each downloaded message, displays subject and saves into "qm mail" folder.
 Leaves messages on server.
Pop3Mail p; MailBee.Message m
p.SetSaveFolder("$desktop$\qm mail")
foreach m p.RetrieveMessages(0x100) ;;to delete messages from server, use flags 2|0x100
	out m.Subject
	p.Save(m)

 Retrieving single message at a time.
 Downloads only headers.
Pop3Mail pp.Connect(0x100 "Another account")
MailBee.Message mm; int i
for i 1 pp.p.MessageCount+1
	mm=pp.p.RetrieveSingleMessageHeaders(i 0) ;;this is MailBee.POP3 object's function
	out mm.Subject
	 pp.p.DeleteMessage(i) ;;use this to delete message from server
pp.Disconnect

 Retrieving and forwarding.
 Note: deletes messages from server.
Pop3Mail pop; SmtpMail smtp; MailBee.Message M
foreach M pop.RetrieveMessages(2)
	smtp.AddMessage("forward@to.com" "" "" 16 "" "" "" "" M)
smtp.Send

 See also: SpamFilter (download from the QM forum).
