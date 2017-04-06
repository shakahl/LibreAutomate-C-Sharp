function $logFile [$activityId] [$description] [flags]

 Remembers start time and parameters.

 logFile - log file that will be used by End().
   Does not limit file size.
   If empty, displays in QM output.
 activityId - something to identify the macro or part of macro. Default: "" - name of the caller macro.
 description - description.
 flags:
   1 - if End() not called explicitly, call it when destroying the variable.
   2 - times with milliseconds.


m_file=logFile
if(empty(activityId)) m_id.getmacro(getopt(itemid 1) 1); else m_id=activityId
m_descr=description
m_flags=flags
m_done=0
m_time.FromComputerTime
