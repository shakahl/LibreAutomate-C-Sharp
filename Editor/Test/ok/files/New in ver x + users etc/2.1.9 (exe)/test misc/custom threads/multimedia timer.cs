int tid=timeSetEvent(3000 10 &mmTimerProc 0 TIME_PERIODIC)
 out tid
 3
 timeKillEvent(tid)
