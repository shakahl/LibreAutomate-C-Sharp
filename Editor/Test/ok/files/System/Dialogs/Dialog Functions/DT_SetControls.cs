 /
function# hDlg [lParam] [!*newVariables]

 Sets data of dialog controls like ShowDialog does.
 Returns: 1 success, 0 failed or no controls specified.

 hDlg - hDlg.
 lParam - must be 0, used only by ShowDialog.
 newVariables - address of variables that contain control data.
   The first variable should be control ids, like "3 5 10". Other variables must be in the order specified in the first variable.
   If the first variable is empty, the function uses the value of the first variable passed to ShowDialog. Then other variables also must be ordered like those passed to ShowDialog. To make this safer, use a user-defined type for control variables.
   If omitted or 0, uses variables passed to ShowDialog (third argument).

 REMARKS
 You can call this function from dialog procedure when you want to set or change control data.
 Does not change bitmaps, icons and dialog title bar text.

 See also: <DT_SetControl> (sets single control), <str.setwintext> (sets raw text).

 EXAMPLE
 str controls="3 4"
 str e3 e4
 e3="a"; e4="b"
 DT_SetControls hDlg 0 &controls


opt noerrorshere

__DIALOG* d; if(lParam) d=+lParam; else d=+GetProp(hDlg +__atom_dialogdata); if(!d) ret
str* p=d.controls
ARRAY(int)& a=d.acid
if newVariables
	p=+newVariables
	if(p[0].len) ARRAY(int) _a; &a=_a; if(!sub_DT.ParseControlsVar(d p a _s)) end ERR_BADARG 8; ret
if(!a.len) ret

int i cid h v2(GetWindowContextHelpId(hDlg)&4)

for i 0 a.len
	str& s=p[i+1]
	cid=a[i]
	if cid
		h=GetDlgItem(hDlg cid)
		if(!h) end F"the dialog doesn't have a control with id {cid}" 8; continue
	else
		if(lParam) s.setwintext(hDlg)
		continue
	
	sub_DT.SetControl(h 0 s iif(lParam 0xC0000000 0x80000000)|v2 d)

ret 1
