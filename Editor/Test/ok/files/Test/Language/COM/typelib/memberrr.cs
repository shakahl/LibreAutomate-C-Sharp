typelib Win32 {CD6E8B50-033A-11D2-B805-EBE0D6230404} 0.5 ;;32-bit Windows API declarations (ANSI), ver 0.5

type AAA Win32.POINTAPI*pp Win32.POINTAPI'p i[Win32.SW_SHOWMINIMIZED]
AAA a
a.p.x=100; a.p.y=200

out "%i %i %i" sizeof(a) a.p.x a.p.y
