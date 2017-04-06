SendMail "test@quickmacros.com" "test" "test" 0 "" "" "" "" "" "test@quickmacros.com"
10
ARRAY(MailBee.Message) am
int nMessages=ReceiveMail("test@quickmacros.com" 0 "" 0 am)
 sample code, shows how to use results
for _i 0 am.len
	MailBee.Message& m=am[_i]
	str subj from
	subj=m.Subject
	from=m.FromAddr
	out "subject=%s, from=%s" subj from
