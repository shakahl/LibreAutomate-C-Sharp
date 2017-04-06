typelib Win32 {CD6E8B50-033A-11D2-B805-EBE0D6230404} 0.5 ;;32-bit Windows API declarations (ANSI), ver 0.5

#ifdef Win32
out "Win32"
#endif

#ifdef Win32.APIBOOL
out "Win32.APIBOOL"
#endif

APIBOOL b=5

#ifdef b
out "b"
#endif
