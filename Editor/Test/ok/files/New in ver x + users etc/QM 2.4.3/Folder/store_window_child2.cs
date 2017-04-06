 \Dialog_Editor

 2
out
int c=TriggerWindow
 outw c
rep
	0.1
	if(GetParent(c)) break
0.1
int p=GetParent(c)
 ARRAY(int) a; int i; child "" "" p 0 "" a; for(i 0 a.len) outw2 a[i]
 out "---"
 int tb=mac("Toolbar57" a[2])
 int tb=mac("Toolbar57" _command)
int tb=mac("Toolbar57" p)
 0.1
 outw2 GetWindow(tb GW_OWNER)
 mac "Toolbar57" GetParent(c)
 int tb=mac("Toolbar57")
  outw tb
 SetWindowLong GetParent(c) GWL_HWNDPARENT tb
 
 
#ret
str dd=
 BEGIN DIALOG
 0 "" 0x94400000  0x8000188 0 0 224 136 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""
 0 "" 0x10C800C8 0x80 0 0 224 136 "Dialog"

int w=TriggerWindow
 RECT r;GetWindowRect w &r; outRECT r
 w=GetParent(w)
 int h=ShowDialog(dd &sub.DlgProc 0 w 1)
int h=CreateWindowEx(WS_EX_TOOLWINDOW "#32770" "" WS_POPUP|WS_CAPTION|WS_SYSMENU|WS_VISIBLE 0 0 100 100 w 0 _hinst 0)
opt waitmsg 1
outw h
outw GetWindow(h GW_OWNER)
 out IsWindowVisible(h)
out IsWindowCloaked(h)
 2
 out IsWindowCloaked(h)
 SetParent h w
wait 0 -WC w
DestroyWindow h

 ShowDialog(dd &sub.DlgProc 0 w)


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 PostMessage hDlg WM_APP 0 0
	 case WM_APP
	 int h=hDlg
	 outw h
	 outw GetWindow(h GW_OWNER)
	 out IsWindowVisible(h)
	 out IsWindowCloaked(h)
	 _i=0
	 outx DwmSetWindowAttribute(h 13 &_i 4)
	 out IsWindowCloaked(h)
	
	
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

#ret
created  1181174  "ImmersiveSplashScreenWindowClass"  ""
created  1573658  "WorkerW"  ""
name  1181174  "ImmersiveSplashScreenWindowClass"  "Calculator"
active  656530  "ApplicationFrameWindow"  ""
created  592260  "ApplicationFrameWindow"  ""
name  656530  "ApplicationFrameWindow"  "Calculator"
visible  592260  "ApplicationFrameWindow"  ""
name  656530  "ApplicationFrameWindow"  "Calculator"
created  722522  "XCPTimerClass"  "XCP"
created  656608  "XAMLMessageWindowClass"  ""
created  526016  "UserAdapterWindowClass"  ""
created  591186  "Windows.UI.Core.CoreWindow"  "Calculator"
visible  591186  "Windows.UI.Core.CoreWindow"  "Calculator"

created  1050538  "ImmersiveSplashScreenWindowClass"  ""
created  1902500  "WorkerW"  ""
name  1050538  "ImmersiveSplashScreenWindowClass"  "Calculator"
active  1836234  "ApplicationFrameWindow"  ""
name  1836234  "ApplicationFrameWindow"  "Calculator"
created  1050278  "ApplicationFrameWindow"  ""
visible  1050278  "ApplicationFrameWindow"  ""
name  1836234  "ApplicationFrameWindow"  "Calculator"
created  918882  "XCPTimerClass"  "XCP"
created  919296  "XAMLMessageWindowClass"  ""
created  525228  "UserAdapterWindowClass"  ""
created  853776  "Windows.UI.Core.CoreWindow"  "Calculator"
visible  853776  "Windows.UI.Core.CoreWindow"  "Calculator"
