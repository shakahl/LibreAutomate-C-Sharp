/exe
out
ARRAY(str) a; GetFilesInFolder a "Q:\My QM\qmm" "*-memleak.qmm" 0 3; a.sort(6|1); str s.gett(a[0] 1 "" 2); s=F"q:\app\qmmacro.exe ''{s}''"
str dm="$program files$\Dr. Memory\bin\drmemory.exe"
str cl=
 -batch
 -no_follow_children 
 -callstack_truncate_below "main,wmain,WinMain,wWinMain,*RtlUserThreadStart,_threadstartex,_initterm,_callthreadstartex"
 -check_handle_leaks
cl.findreplace("[]" " ")
RunConsole2 F"{dm} {cl} -- {s}" _s
 -callstack_max_frames 8
 -no_check_uninitialized 
 -show_reachable

DrMemoryProcessResults _s 4
out _s
OutputScrollToTop

 BEGIN PROJECT
 main_function  Macro1997
 exe_file  $my qm$\memleaks_run_qm.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {26BA6F6A-93AC-4DEA-B757-0C80A70CA01A}
 END PROJECT
