_s=
 pop_server mail.quickmacros.com
 pop_port 110
 pop_user support@quickmacros.com
 pop_password 2A16EFC757EF5389C86FE3F3B1FCE48E07
 pop_auth 0
 pop_secure 0
 pop_timeout 30
ARRAY(MailBee.Message) am
ReceiveMail(_s 0 "" 0 am)
 sample code
for _i 0 am.len
	MailBee.Message& m=am[_i]
	str subj from
	subj=m.Subject
	from=m.FromAddr
	out "subject=%s, from=%s" subj from
