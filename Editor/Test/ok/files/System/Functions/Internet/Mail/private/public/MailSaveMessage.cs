 /
function# MailBee.Message&m $emlfile

str s=m.RawBody
s.setfile(emlfile); err ret
ret 1
 note: don't use m.SaveMessage because it fails if path is Unicode
