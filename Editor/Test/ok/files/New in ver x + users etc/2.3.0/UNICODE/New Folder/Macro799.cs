out
Pop3Mail p.Connect(0x100)
p.SetSaveFolder("$desktop$\ąč ﯔﮥ qww")
 p.SetSaveFolder("$desktop$\new folder")
MailBee.Message m
foreach m p.RetrieveMessages(0x100) ;;to delete messages from server, use flags 2|0x100
	 out m.Subject
	p.Save(m)
	 break
	 out "-------"
