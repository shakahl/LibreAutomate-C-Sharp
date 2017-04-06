out
 int w=win("Replace" "#32770")
int w=win("Dialog" "#32770")
ARRAY(int) a
child("" "" w 0 0 0 a)

str s.all(1000) s2.all(1000) s3
BSTR b.alloc(1000) b2.alloc(1000)

int i
for i 0 a.len
	int h=a[i]
	out "----"
	outw h
	Q &q
	int r1=GetWindowText(h s 1000)
	Q &qq
	int r2=InternalGetWindowText(h b 1000)
	Q &qqq
	int r3=SendMessage(h WM_GETTEXT 1000 s2)
	 int r3; if(!SendMessageTimeout(h WM_GETTEXT 1000 s2 SMTO_ABORTIFHUNG|SMTO_BLOCK 1000 &r3)) s2[0]=0
	 int r3=SendMessageW(h WM_GETTEXT 1000 b2.pstr); s2.ansi(b2)
	Q &qqqq
	 Acc ac=acc(h); s3=ac.Name
	 Q &qqqqq
	outq
	
	if(r1) out "gwt: %s" s.lpstr; else out "gwt:"
	if(r2) out "igwt: %s" _s.ansi(b); else out "igwt:"
	if(r3) out "sm: %s" s2.lpstr; else out "sm:"
	 out "acc: %s" s3
	
	