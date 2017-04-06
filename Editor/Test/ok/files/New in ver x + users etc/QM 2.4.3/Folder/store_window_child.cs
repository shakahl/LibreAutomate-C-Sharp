\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x10C80AC8 0x0 0 0 224 136 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

int h=ShowDialog(dd &sub.DlgProc 0 0 1)
outw h
int w=TriggerWindow
SetParent h w
opt waitmsg 1
wait 0 -WC w
out IsWindow(w)


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
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
