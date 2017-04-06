 /
function# ARRAY(__REGEMAILACCOUNT)&a

 Gets array of account names and key names, and returns default account index or -1.

str s ss sk sdef
int i idef=-1

if(rget(sdef "QM default mail account" RK_IAM)<2) rget(sdef "Default Mail Account" RK_IAM)

a.redim
foreach sk RK_IAMA FE_RegKey 0 1
	s.from(RK_IAMA "\" sk)
	if(!rget(ss "POP3 Server" s)) continue
	if(rget(ss "Account Name" s)<2) ss=sk
	__REGEMAILACCOUNT& r=a[]
	r.name=ss; r.keyname=sk
	if(idef<0 and sk~sdef) idef=i
	i+1
ret idef
