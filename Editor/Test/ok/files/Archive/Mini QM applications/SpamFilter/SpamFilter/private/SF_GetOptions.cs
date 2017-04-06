lpstr rk="\SpamFilter"
 g1
rget o.flags "flags" rk 0 8
rget o.period "period" rk 0 10; if(o.period<1) o.period=1
rget o.period0 "period0" rk 0 10
rget o.nlines "nlines" rk 0 1000

rget o.accountsr "accounts" rk 0 "<default>"
if(o.accountsr~"<all>")
	o.accounts.all
	ARRAY(__REGEMAILACCOUNT) a
	MailGetAccounts(a)
	for(_i 0 a.len) o.accounts.addline(a[_i].name)
else o.accounts=o.accountsr

rget o.sound "sound" rk
rget o.ff "ff" rk

str& s=o.mailapp
rget s "mailclient" rk
if(!s.len)
	rget s "" "mailto\shell\open\command" HKEY_CLASSES_ROOT
	if(s.beg("''")) s.gett(s 0 "''"); else s.replacerx(" +[/\-].+")
