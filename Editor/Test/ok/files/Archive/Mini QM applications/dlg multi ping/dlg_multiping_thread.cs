 \
function hDlg row $dest period

rep
	int t ttl
	rep 3
		if(!__multiping_started) ret
		t=Ping(dest ttl)
		if(t) break
		0.3
	if(!t) ttl=0
	SendMessage hDlg WM_APP row ttl<<16|t
	rep(period) 1; if(!__multiping_started) ret
