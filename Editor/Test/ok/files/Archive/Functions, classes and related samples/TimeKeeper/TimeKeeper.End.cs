function [$comments]

 Calculates time elapsed since Begin() and writes to the log file.

 comments - comments. For example, results of the activity (success, failed, etc).


DateTime t.FromComputerTime

str s sd st1 st2 ste
sd=m_time.ToStr(1)
_i=iif(m_flags&2 10 6)
st1=m_time.ToStr(_i)
st2=t.ToStr(_i)
ste=TimeSpanToStr(t-m_time m_flags&2)

s=
F
 Activity Id: {m_id}
 Description: {m_descr}
 Date: {sd}
 Start Time: {st1}
 End Time: {st2}
 Elapsed Time: {ste}
 Comments: {comments}
 
if(m_file.len) s.setfile(m_file -1 -1 1)
else out s
m_done=1
