 Functions of Pop3Mail class can be used to retrieve email messages.
 See also: <Email help>, <Email samples>.

 EXAMPLE
 Downloads messages from default email account, and saves. Leaves messages on server.
Pop3Mail p; MailBee.Message m
p.SetSaveFolder("$desktop$\pop")
foreach m p.RetrieveMessages(0x100) ;;to delete messages from server, use flags 2|0x100
	out m.Subject
	p.Save(m)
