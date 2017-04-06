int w=win("Calculator" "ApplicationFrameWindow") ;;4 (User) 2 (medium)
w=child("Calculator" "Windows.UI.Core.CoreWindow" w) ;;4 (User) 1 (Low)
out GetProcessUacInfo(w)
out GetProcessIntegrityLevel(w)
