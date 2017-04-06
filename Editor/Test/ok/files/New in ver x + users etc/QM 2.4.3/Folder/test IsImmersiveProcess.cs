 int w=win("Calculator" "ApplicationFrameWindow" "" 0x0 "cClass=Windows.UI.Core.CoreWindow")
 w=child("Calculator" "Windows.UI.Core.CoreWindow" w) ;; 'Calculator'
 int w=win("Quick Macros - ok - [Macro2591]" "QM_Editor")
 int w=win("Registry Editor" "RegEdit_RegEdit")
 int w=win("" "Shell_TrayWnd")
 int w=win("" "WorkerW")
 int w=win("My QM" "CabinetWClass")
outw w

dll- user32 #IsImmersiveProcess hProcess

__HProcess p
out p.Open(w PROCESS_QUERY_LIMITED_INFORMATION)
out IsImmersiveProcess(p)
