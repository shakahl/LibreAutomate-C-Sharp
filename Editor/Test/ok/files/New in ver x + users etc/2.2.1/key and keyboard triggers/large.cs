/exe
 act "Notepad"
out
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_LOWEST
out "macro"
 opt slowkeys 1
 opt keynosync 1
spe
 spe 100
key (0.2) CaX (0.2)
 ret
 _s.getmacro("Macro512")
_s.getfile("$qm$\winapiqm.txt")
_s.fix(1000)
 _s.getl(_s 1)
int t1=perf
 'F6F6F6
 key (50)
 key (0x40588)(0x40588)(0x40588)
 'Y "kk kk kk kk kk kk kk kk k k k k k k k k k k k k k k k k k k k k k k k k  k k k k k k k  k k k k k k k k k k k k k k k k  k k k k  k k k k k k  k k k k k k k k  k k k k k k k k k k m"
key (_s)
 outp _s
 'YkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkmCSAq
 'kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkmY
 'kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk
 '"if(!LLh && /*!IsConsoleWindow(GetForegroundWindow()) &&*/ SK_WaitForKeyReceivedByHook(10000)) wfk=3; //for console, ati.attached is 0. If same thread, ati.attached also is 0, which is good because otherwise would always wait 10-20 ms."
 '"if(!LLh && /*!IsConsoleWindow( GetForegroundWindow( )) ."
 act _hwndqm
 'Vkk
int t2=perf
 out t2-t1

key "[]OK"
 key+ kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk
 key kYV

 key qwertyuiopasdfghjkCSLX


 kk kk
 kk kk kk
 kk kk kk kk kk kk






 BEGIN PROJECT
 main_function  Macro482
 exe_file  $my qm$\Macro482.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {D4997896-0722-43E8-8F71-D981BC0C53C0}
 END PROJECT
