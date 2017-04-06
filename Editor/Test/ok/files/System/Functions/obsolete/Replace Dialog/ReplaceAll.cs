
 Shows Replace dialog.


type ___REPLACEALL ~controls ~Edit3 ~Edit4 ~Button5 ~Button6 ~Button7 ~Button8 ~Button9
___REPLACEALL r
r.controls = "3 4 5 6 7 8 9"
r.Edit3.getsel
r.Button7=1
int+ ___replaceoptions; if(___replaceoptions=0) ___replaceoptions=0x103
if(___replaceoptions&1) r.Button6=1
if(___replaceoptions&2) r.Button5=1
if(___replaceoptions&4) r.Button9=1

if(r.Edit3.len)
	if(findc(r.Edit3 13)>=0) r.Edit3.all; r.Button7.all; r.Button8="1"
	else if(r.Edit3[0]>32) r.Edit3.rtrim

ShowDialog("ReplaceDlg" &ReplaceDlg &r win)
