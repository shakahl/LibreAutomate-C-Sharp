 /
function hDlg

___EA- dA
SendDlgItemMessage hDlg 10 __TWM_SETFLAGS 0x200 1 ;;don't declare var; the control unsets the flag
___EA_CONTROLS& c=DT_GetControls(hDlg)
str s
if(!EA_Format(c s 1)) bee; ret

EA_Info hDlg
sub_to.Test hDlg s dA.hwnd 1 48

#ret
function hDlg ~statement hwnd
int isFF
sub_to.Test_ActWin8 hwnd
#ret

if !a.a
	str st
	if isFF
		st=
 Possible reasons and solutions:
 1. Some property does not match. Try to change some values.
 2. If searching not in web page, try to uncheck 'as Firefox node'.
 3. Try to capture other object and use Navigate.
	else
		st=
 Possible reasons and solutions:
 1. Some property does not match. Try to clear or change some values.
 2. The object or some object in the path is invisible. Try to check 'Search -> + invisible objects'.
 3. Try to check 'Search -> + useless objects'.
 4. Incorrect implementation of accessible objects in the window. Try to clear Class or some other value.
 5. Try to capture other object and use Navigate.
	sub_sys.MsgBox hDlg F"{statement}[][]{st}" "Object not found" "!"; end

int i
err-
__OnScreenRect osr
if(a.State&STATE_SYSTEM_INVISIBLE=0)
	__MinimizeDialog m; if(GetWinStyle(hDlg 1)&WS_EX_TOPMOST) m.Minimize(hDlg)
	if(!hwnd) hwnd=GetAncestor(child(a) 2)
	act hwnd; 0.25
	EA_Rect osr 0 a; 0.25
	for(i 1 6) osr.Show(i&1*3); 0.25
	act hDlg
err+
