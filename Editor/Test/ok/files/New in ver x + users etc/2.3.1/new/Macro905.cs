dll "qm.exe"
	CsEnter
	CsLeave
	!CsEnterT timeout
out

int+ g_dontleave=1
double wt=0.1

mac "th1"
wait wt
mac "th2"
wait wt
mac "th3"
