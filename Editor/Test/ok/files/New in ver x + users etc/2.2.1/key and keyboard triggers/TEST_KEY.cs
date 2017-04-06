function [$wintext]
if(len(wintext)) act win(wintext "" "" findc(wintext '*')>=0)
ifa(_hwndqm) ret
out
 opt slowkeys 1
opt keysync 0
 opt keymark 1
 srand GetTickCount; tls5=rand&1; out tls5
spe 100
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_ABOVE_NORMAL
 MinimizeProcessMemory win; 1

 int t0=timeGetTime
 key AT
 out timeGetTime-t0-spe
 ret

key CaX (0.1)
out "----"
 out "---- current cpu = %i" NtGetCurrentProcessorNumber
_s.getmacro("Macro512")
 _s.getmacro("ideas"); out _s.len
 _s.getl(_s 1)
 _s.fix(100)
 _s.lcase
 _s="aAsSdDfFgGhHjJkKlL[]"

int t1=timeGetTime

key (_s)

 spe 0
  opt keysync 1
 int i; str c
 for(i 0 _s.len)
	 if(_s[i]!=13) key (c.get(_s i 1)); else key Y; i+1
	  Sleep 0

 wait 0 K C
int t2=timeGetTime
int t=t2-t1-spe
out t
 OnScreenDisplay _s.from(t)
 zw win


 ------------------------------------------------------------


 int tf1=timeGetTime
 zw WaitForFocus
 int tf2=timeGetTime
 out tf2-tf1

 MinimizeProcessMemory win
 key qwertyuiopasdfghjklzxcvbnmY
 key a; ret

 _s.getmacro("ideas")
 _s.findreplace("[]" " ")
 _s.findreplace(" " "[]")
 _s.getfile("$qm$\winapiqm.txt"); _s.fix(10000)
 _s.set('a')
 _s="///////////////////////////////////////////////////////////////////////////"
 key aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa; ret
 'F6F6F6
 key (50)
 key (0x40588)(0x40588)(0x40588)
 'Y "kk kk kk kk kk kk kk kk k k k k k k k k k k k k k k k k k k k k k k k k  k k k k k k k  k k k k k k k k k k k k k k k k  k k k k  k k k k k k  k k k k k k k k  k k k k k k k k k k m"
 outp _s
 'YkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkmCSAq
 'kkkkkkk Cb kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkmY
 'kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk
 '"if(!LLh && /*!IsConsoleWindow(GetForegroundWindow()) &&*/ SK_WaitForKeyReceivedByHook(10000)) wfk=3; //for console, ati.attached is 0. If same thread, ati.attached also is 0, which is good because otherwise would always wait 10-20 ms."
 '"if(!LLh && /*!IsConsoleWindow( GetForegroundWindow( )) ."
 act _hwndqm
 'Vkk

 act "Notepad"
 key "[]OK"
 key+ kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk
 key kYV

 key qwertyuiopasdfghjkCSLX
