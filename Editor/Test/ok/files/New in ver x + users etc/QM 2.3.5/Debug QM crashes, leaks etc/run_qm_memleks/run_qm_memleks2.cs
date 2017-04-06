 \
int w=win("" "QM_Editor")
if w
	men 101 w ;;Exit Program
	wait 0 WP w

str s="q:\app\qm.exe"
str dm="$program files$\Dr. Memory\bin\drmemory.exe"
str cl=
 -batch
 -no_follow_children 
 -callstack_truncate_below "main,wmain,WinMain,wWinMain,*RtlUserThreadStart,_threadstartex,_initterm,_callthreadstartex"
 -check_handle_leaks
cl.findreplace("[]" " ")
 -show_reachable

mac "run_qm_memleks3" ;;show QM window when CPU drops
RunConsole2 F"{dm} {cl} -- {s}" _s

DrMemoryProcessResults _s 4
ShowText "" _s

 -check_handle_leaks
 -no_check_gdi 
