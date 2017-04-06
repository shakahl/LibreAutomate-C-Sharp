out
ARRAY(int) a
opt hidden 1
win "" "" "" 0 "" a
int i; str s ss
for i 0 a.len
	sub.Test(a[i])


#sub Test
function w
if(!WinTest(w "ApplicationFrameWindow[]Windows.UI.Core.CoreWindow")) ret
 if(!WinTest(w "ApplicationFrameWindow")) ret
 if(!IsWindowVisible(w)) ret
out "---"
outw2 w
if(!WinTest(w "ApplicationFrameWindow")) ret
outw sub.GetWindowsStoreAppFrameChild(w)


#sub GetWindowsStoreAppFrameChild
function# hwnd

 On Win10+, if hwnd is "ApplicationFrameWindow", returns the real app window "Windows.UI.Core.CoreWindow" hosted by hwnd.
 If hwnd is minimized, cloaked (eg on other desktop) or the app is starting, the "Windows.UI.Core.CoreWindow" is not its child. Then searches for a top-level window with the same name as of hwnd. It is unreliable, but MS does not provide API for this.
 Info: "Windows.UI.Core.CoreWindow" windows hosted by "ApplicationFrameWindow" belong to separate processes. All "ApplicationFrameWindow" windows belong to a single process.


 g1
if(_winver<0xA00 or !WinTest(hwnd "ApplicationFrameWindow")) ret
int c=FindWindowExW(hwnd 0 L"Windows.UI.Core.CoreWindow" 0)
if(c) ret c
int retry; if(retry) ret

_s.getwintext(hwnd); if(!_s.len) ret
BSTR b=_s

rep
	c=FindWindowExW(0 c L"Windows.UI.Core.CoreWindow" b) ;;I could not find API for it
	if(!c) break
	if(IsWindowCloaked(c)) ret c ;;else probably it is an unrelated window

retry=1; goto g1 ;;maybe SetParent called while we searched for top-level window etc, eg when starting the app or switching Win10 desktops

err+
