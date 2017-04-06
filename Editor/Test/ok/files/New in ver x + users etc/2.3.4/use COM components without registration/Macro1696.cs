 MailBee.POP3 x._create

Pop3Mail p; MailBee.Message m
p.SetSaveFolder("$desktop$\qm mail")
foreach m p.RetrieveMessages(0x100) ;;to delete messages from server, use flags 2|0x100
	out m.Subject
	p.Save(m)
