out

 download qmtul.exe
_s="$qm$\qmtul.exe"
IntGetFile "http://www.quickmacros.com/dev/qmtul.exe" _s 16

 run
int t1=timeGetTime
out "EC: %i" run(_s "" "" "" 0x400)
out "time: %i" timeGetTime-t1

mes "Copy everything from QM output, and post to forum."
