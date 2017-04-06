int h=mac("thread_end")

 atend EndThread "thread_end" ;;Error in Macro1497:  bad number or type of arguments.

2

 EndThread "thread_end"
EndThread +qmitem("thread_end")
 EndThread "" h
 EndThread "thread_end" h
 EndThread "thread_end2" h

 QMTHREAD qt
 GetQmThreadInfo h &qt
 EndThread "" qt.threadhandle
 EndThread "" qt.threadid 1
 EndThread "" qt.tuid 2

 EndThread "thread_end" 0 0
 EndThread "thread_end" 0 4
 EndThread "thread_end" 0 8
 2
