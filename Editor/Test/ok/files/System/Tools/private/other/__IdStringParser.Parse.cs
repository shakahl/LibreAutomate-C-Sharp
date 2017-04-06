function hDlg $s

 Parses string of control ids and gets hwnd's into member array a.

 hDlg - dialog. If 0, gets ids instead of hwnds.
 s - list of ids, like "3 5 10".
   Groups of ids can be specified like 3-5, without spaces.
   An id or group can have a prefix operator:
     + the control is not direct child.
     - set flag 1.
   If several operators used, they must be in the above order.
   Can be used id 0 for hDlg.

 If control not found or incorrect string, adds string to warnings.
 This func is safe to use with user-entered strings, but caller should display warnings.

a=0; warnings.all

int i ci ci2 h f all k
ARRAY(lpstr) g
tok s g -1 " "

for i 0 g.len
	s=g[i]
	f=0; all=0
	if(s[0]='+') s+1; all=1
	if(s[0]='-') s+1; f|1
	
	ci=val(s 0 k); if(!k) goto ge
	s+k
	if(s[0]='-') ci2=val(s+1 0 k); if(!k) goto ge
	else ci2=ci
	
	for ci ci ci2+1
		 out ci
		if(!hDlg) h=ci
		else if(ci) h=id(ci hDlg !all); if(!h) warnings.formata("Control %i does not exist. " ci); continue
		else h=hDlg
		__HWNDFLAGS& r=a[]; r.hwnd=h; r.flags=f
ret
 ge
a=0
warnings.formata("Incorrect format: ...%s. Must be like ''3 5 10-15''." s)
