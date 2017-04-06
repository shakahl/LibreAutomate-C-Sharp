 note: currently don't need to run this for x.x.x.X. Only for x.x.X.

 Also update:
 $qm$\web\version.php
 $qm$\HtmlHelp\QM_Help\IDH_WHATSNEW2.html
 $qm$\web\index.html
 $qm$\web\download.html
 $qm$\quickm21.iss (x.x.X only)
 PAD: http://publisher.appvisor.com/


int v=QMVER
out "Current version is 0x%X" v

str files=
 $qm$\HtmlHelp\QM_Help\IDH_OVERVIEW.html

str s ss f ff
int n
foreach f files
	if(!f.len) continue
	ff.searchpath(f); if(!ff.len) bee; out "File not found: %s" f; continue
	s.getfile(ff); if(!s.len) bee; out "File not found: %s" f; continue
	
	n=s.replacerx("2\.\d{1,2}\.\d{1,2}" _qmver_str 2)
	
	if(n) s.setfile(ff)
	out "%s    %i" ff n
