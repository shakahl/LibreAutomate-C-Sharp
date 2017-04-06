 /
function $projectName ;;projectName: if "", compiles current

int w=win("Microsoft Visual Studio" "wndclass_desked_gsk")
act w

if !empty(projectName)
	int c=child("Solution Explorer" "SysTreeView32" w) ;;outline
	Acc a.Find(w "OUTLINEITEM" projectName "class=SysTreeView32[]value=1" 0x1015)
	a.Select(2)
	a.Mouse(1 -5 5) ;;because Select does not make the project active etc

 click 'Build project' button. Tried hotkey, but it often does not work correctly. Acc slow.
scan "image:h249F12F6" child("Menu Bar" "MsoCommandBar" w) 0 1|2|16 8 ;;menu bar 'Menu Bar'
lef

 click the edit pane to activate, because act etc does not work normally
c=child("" "VsEditPane" w 0x0 "id=42")
lef 0.995 0.995 c

mou

 Call this from exe. Pin the exe to the taskbar.
 Currently using function Build_app for this.
 Don't use VC macros because:
   Slowly starts, uses separate process.
   Usually locks qmhook32.dll.
   Need 2 toolbar buttons. The buttons are not disabled while building.
   Does not restore focus to the code editor.
   