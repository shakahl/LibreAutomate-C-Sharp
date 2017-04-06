 /
function# hDlg on mice [silent]

RAWINPUTDEVICE Rid
Rid.usUsagePage =1
Rid.usUsage = iif(mice 2 6)
if(on)
	Rid.dwFlags=RIDEV_INPUTSINK
	Rid.hwndTarget=hDlg
else Rid.dwFlags=RIDEV_REMOVE

int r=RegisterRawInputDevices(&Rid 1 sizeof(RAWINPUTDEVICE))

if(!r and !silent)
	_s.dllerror
	mes "Failed to register or unregister %s, %s" "Keyboard detector" "x" iif(mice "mouses" "keyboards") _s
ret r
