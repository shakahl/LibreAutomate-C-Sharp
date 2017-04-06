function flags [idleMinutes] [idleWatchMinutes] ;;flags: 0x10 start only if idle, 0x20 end task if computer ceases to be idle, 0x40 don't start if on batteries, 0x80 end task if battery mode begins, 0x800 start again when idle state resumes, 0x1000 wake computer

 Sets task idle time and power options as in task properties Settings tab.

 flags - see above.
 idleMinutes, idleWatchMinutes - as in task properties Settings tab. Applied only if flag 0x10 used.


if(flags&0x10) m_task.SetIdleWait(idleMinutes idleWatchMinutes)
m_task.GetFlags(&_i); outx (_i&0x2206)|(flags~0x2206); m_task.SetFlags((_i&0x2206)|(flags~0x2206))

err+ end _error
