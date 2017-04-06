 /exe
if(!AttachConsole(ATTACH_PARENT_PROCESS)) ErrMsg 1 _s.dllerror ;;note: this fails if the exe is not running from cmd.exe (DOS prompt)

 using C console functions
_cputs "[]Qm exe here.[]" ;;simple string
str s.time("%X")
_cprintf "Now is %s[]" s ;;formatted like out

 using Windows console functions
int hStdout=GetStdHandle(STD_OUTPUT_HANDLE)
SetConsoleTextAttribute hStdout FOREGROUND_GREEN
s.format("Press Enter to continue.[]")
WriteFile hStdout s s.len &_i 0
SetConsoleTextAttribute hStdout FOREGROUND_RED|FOREGROUND_GREEN|FOREGROUND_BLUE ;;white

 if exe launches other threads, it must wait until they'll end
 because if exe ends immediately, it terminates these threads
ARRAY(int) threads
threads[]=mac("Function64") ;;launch second thread
threads[]=mac("Function67") ;;launch third thread
WaitForMultipleObjects threads.len &threads[0] 1 INFINITE

 all threads can use _cprintf etc too

ret 400 ;;exit code

 BEGIN PROJECT
 main_function  Macro314
 exe_file  $desktop$\Macro314.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {7AE0B595-B17A-4D4F-8821-DA57351255D9}
 END PROJECT
