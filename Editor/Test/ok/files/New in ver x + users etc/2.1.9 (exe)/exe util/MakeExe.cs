ClearOutput

int idd=qmitem
int iddtest=qmitem("dialog15")
if(idd!=iddtest)
	int re=id(2210 _hwndqm)
	str s.getwintext(re)
	mac+ iddtest
	act re
	key Ca
	0.2
	s.setsel
	0.2
	

men 33233 _hwndqm ;;Make exe ...
but 1 wait(5 WA "Make exe")
