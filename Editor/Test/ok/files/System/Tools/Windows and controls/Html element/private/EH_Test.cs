 /
function hDlg

___EH- dH
SendDlgItemMessage hDlg 10 __TWM_SETFLAGS 0x200 1 ;;don't declare var; the control unsets the flag
___EH_CONTROLS& c=DT_GetControls(hDlg)
str s
if(!EH_Format(c s 1)) bee; ret

sub_to.Test hDlg s dH.hwnd 1 6

#ret
function hDlg ~statement hwnd
sub_to.Test_ActWin8 hwnd
#ret

if(!e) sub_sys.MsgBox hDlg statement "Element not found" "!"; end

err-
__OnScreenRect osr
int i; RECT r
e.GetRect(r); err ret
__MinimizeDialog m; if(GetWinStyle(hDlg 1)&WS_EX_TOPMOST) m.Minimize(hDlg)
act hwnd; 0.25
for(i 0 6) osr.Show(i&1*3 r); 0.25
act hDlg
err+
