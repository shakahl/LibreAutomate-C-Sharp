 /
function# hDlg message

___EA- dA
Acc a
__MinimizeDialog md
__OnScreenRect osr

sel message
	case WM_LBUTTONDOWN
	md.Minimize(hDlg)
	__Drag d.Init(hDlg 1)
	rep() EA_Rect osr; if(!sub_to.DragTool_Loop(d)) break
	if(!d.dropped) ret
	
	case WM_RBUTTONDOWN
	str s="{+}[]1 Capture smallest object"
	if(EA_Smallest(3)) s.findreplace("[]1 " "[]1.2 " 4)
	sel sub_to.DragTool_Menu(hDlg s 5)
		case 102 int shiftCapture=1
		case 1 EA_Smallest 1 hDlg
	
	if(!shiftCapture) ret
	opt waitmsg 1
	md.Minimize(hDlg)
	rep
		0.01
		ifk(S) break; else ifk(Z) ret
		POINT p pp; xm p; if(memcmp(&p &pp 8)) pp=p; else continue
		EA_Rect osr

EA_Rect osr a
if(!shiftCapture) md.Restore ;;I don't remember why restore now (maybe because sometimes getting tree is very slow and the user would not know what is going on), but don't do it when Shift-capturing, because it may destroy the target window eg a popup menu before we get the tree

int w=GetAncestor(child(a) 2); if(!w) ret
if(w=dA.hwnd and EA_Select(hDlg a)>=0) ret
dA.hwnd=w
dA.ai=a
dA.isFilled=0
EA_Proc hDlg !but(9 hDlg)
if(!dA.isFilled) EA_Fill hDlg a
if(_winver>=0x603 and DpiIsWindowScaled(w)) EA_Info hDlg "<>This window is DPI-scaled. You may want to disable scaling to get correct coordinates of objects. <help #IDP_DPI>Read more</help>."
