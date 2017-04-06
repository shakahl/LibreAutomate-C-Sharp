 /
function hDlg [retVal] ;;retVal: 0 on Cancel, nonzero on OK

__DIALOG* d=+GetProp(hDlg +__atom_dialogdata); if(!d) ret
if(d.flags2&8) ret ;;already called
d.flags2|8

if(retVal and d.flags&1=0 and d.flags2&4=0) ;;OK, modal, not called
	d.flags2|4
	DT_GetControls(hDlg)

if(RealGetParent(hDlg)) ret

if(d.flags&0x81)
	if(d.flags&1=0) PostMessage 0 RegisterWindowMessage("WM_QM_ENDDIALOG") retVal 0 ;;end message loop
	DestroyWindow hDlg
else EndDialog hDlg retVal
