function# MailBee.Message&m $sf

str s ss
if(!MailSaveMessage(m sf)) ret

 add bcc. Note: current MailBee version does not save bcc, but this may change in future.
s=m.BCCAddr
if(s.len)
	ss.getfile(sf)
	ss.findreplace("[][]" s.from("[]Bcc: " s "[][]") 4)
	ss.setfile(sf); err
ret 1
