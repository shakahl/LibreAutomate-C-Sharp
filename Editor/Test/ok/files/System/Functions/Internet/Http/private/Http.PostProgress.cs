function# action ___POSTPROGRESS&pp [$fname] [fsize] [fwritten]

if m_hdlg or m_fa
	str sa
	sel action
		case 1
		if(m_hdlg and !pp.wrtotal) sa="(uploading files)"
		if(!Progress(pp.nbtotal pp.wrtotal 0)) ret 1
		case 3
		if(m_hdlg) sa="(downloading)"
	if(sa.len) sa.setwintext(id(3 m_hdlg))

if(!pp.fa) ret

 not too frequent
if action=1
	int-- tp
	if(GetTickCount-tp<100 and fwritten and fwritten!=fsize) ret

if(call(pp.fa action pp.nbtotal pp.wrtotal fname fsize fwritten pp.fparam)) lasterror="cancel"; ret 1

if(action=1) tp=GetTickCount
