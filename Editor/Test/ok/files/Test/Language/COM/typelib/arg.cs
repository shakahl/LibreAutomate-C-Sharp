typelib Win32 {CD6E8B50-033A-11D2-B805-EBE0D6230404} 0.5 ;;32-bit Windows API declarations (ANSI), ver 0.5

dll user32 [SetCursorPos]SCP Win32.POINTAPI'p
dll user32 [GetCursorPos]GCP Win32.POINTAPI*p

Win32.POINTAPI p pp
p.x=300; p.y=0
SCP p

GCP &pp
out "%i %i" pp.x pp.y
