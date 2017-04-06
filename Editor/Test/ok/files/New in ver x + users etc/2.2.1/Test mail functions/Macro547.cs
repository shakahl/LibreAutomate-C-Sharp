ARRAY(MailBee.Message) a; int n
n=ReceiveMail("qmgindi@gmail.com" 0x100 "" 0 a)
 sample code
for _i 0 a.len
	MailBee.Message& m=a[_i]
	str subj from
	subj=m.Subject
	from=m.FromAddr
	out "subject=%s, from=%s" subj from
