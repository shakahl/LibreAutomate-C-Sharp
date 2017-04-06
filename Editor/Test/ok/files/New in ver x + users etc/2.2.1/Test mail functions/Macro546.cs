out
Pop3Mail p.Connect(0x100 "qmgindi@gmail.com")
MailBee.Messages mm=p.RetrieveMessages(0)
out mm.Count
int i
for i 0 mm.Count
	MailBee.Message m=mm.Item(i)
	out m.Subject
	