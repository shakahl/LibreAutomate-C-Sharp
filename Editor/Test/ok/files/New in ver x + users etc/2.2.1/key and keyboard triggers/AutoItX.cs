typelib AutoItX3Lib {F8937E53-D444-4E71-9275-35B64210CC3B} 1.0
AutoItX3Lib.AutoItX3 a._create

_s.getmacro("Macro512")
_s.findreplace("[]" "[13]")

 SetThreadPriority GetCurrentThread THREAD_PRIORITY_ABOVE_NORMAL

 a.WinActivate(
a.Send("^a {DELETE}" 0)
a.AutoItSetOption("SendKeyDelay" 1)
a.AutoItSetOption("SendKeyDownDelay" 1)
int t1=perf
a.Send(_s 1)
int t2=perf
out t2-t1/1000
