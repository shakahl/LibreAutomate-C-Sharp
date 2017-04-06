 /
function'str* hDlg [!*newVariables]

 Gets data from controls and stores into dialog variables.
 Returns address of the first variable.

 newVariables - address of str variables to populate.
   The first variable should be control ids, like "3 5 10". Other variables must be in the order specified in the first variable.
   If the first variable is empty, the function uses the value of the first variable passed to ShowDialog. Then other variables also must be ordered like those passed to ShowDialog. To make this safer, use a user-defined type for control variables.
   If omitted or 0, populates variables passed to ShowDialog (third argument).
   For modeless dialogs, newVariables should not be 0, because initial variables probably already don't exist.

 REMARKS
 You can call this function from dialog procedure when you want to get data from controls without closing the dialog.
 Gets control data in the same format as <help>ShowDialog</help> gets to populate dialog variables on OK.

 See also: <DT_GetControl> (gets single control), <str.getwintext> (gets raw text).

 EXAMPLES
  populates all control variables passed to ShowDialog, like OK button does. To access them, uses the same user-defined type as before ShowDialog.
 MY_DIALOG_CONTROLS& d=+DT_GetControls(hDlg)
 out d.e3

  stores all control variables in a new variable of the same user-defined types as before ShowDialog. This code can be used in modeless dialogs too.
 MY_DIALOG_CONTROLS d
 DT_GetControls(hDlg &d)
 out d.e3

  populates all control variables passed to ShowDialog. To access them, uses #sub with attribute v.
 str dd=
  BEGIN DIALOG
  0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
  3 Edit 0x54030080 0x200 8 8 96 12 ""
  4 Button 0x54012003 0x0 8 24 48 10 "Check"
  5 Button 0x54032000 0x0 8 40 48 14 "Get controls"
  1 Button 0x54030001 0x4 116 116 48 14 "OK"
  2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
  END DIALOG
  DIALOG EDITOR: "" 0x2040108 "*" "" "" ""
 
 str controls = "3 4"
 str e3 c4Che
 if(!ShowDialog(dd &sub.DlgProc &controls)) ret
 
 
 #sub DlgProc v
 function# hDlg message wParam lParam
 
 sel message
	 case WM_INITDIALOG
	 case WM_DESTROY
	 case WM_COMMAND goto messages2
 ret
  messages2
 sel wParam
	 case IDOK
	 case 5 ;;Get controls
	 DT_GetControls hDlg
	 out e3
	 out c4Che
 ret 1


opt noerrorshere

__DIALOG* d=+GetProp(hDlg +__atom_dialogdata)
str* p=d.controls
ARRAY(int)& a=d.acid
if newVariables
	p=+newVariables
	if(p[0].len) ARRAY(int) _a; &a=_a; if(!sub_DT.ParseControlsVar(d p a _s)) end ERR_BADARG 8; ret p
if(!a.len) ret p

int i cid h v2(GetWindowContextHelpId(hDlg)&4)
lpstr controls=p[0]

for i 0 a.len
	str& s=p[i+1]
	cid=a[i]
	if(!cid) continue
	h=GetDlgItem(hDlg cid)
	if(!h) end F"the dialog doesn't have a control with id {cid}" 8; s.all; continue
	
	sub_DT.GetControl(h 0 s 0x80000000|v2)
ret p
