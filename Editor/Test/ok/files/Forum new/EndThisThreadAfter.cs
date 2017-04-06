 /
function ^timeS

 Schedules to end this thread after timeS seconds.
 If it is the first thread in .exe, then .exe process also will end.

 EXAMPLE
 EndThisThreadAfter 3
 mes "macro"


QMTHREAD qt; GetQmThreadInfo 0 qt
mac "sub.ET" "" timeS qt.threadhandle


#sub ET
function ^timeS ht
wait timeS
EndThread "" ht
