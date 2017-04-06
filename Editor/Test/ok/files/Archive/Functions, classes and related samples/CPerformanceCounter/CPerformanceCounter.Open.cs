function $counter

 Opens pdh query and adds 1 performance conter.
 Error if fails.

 counter - counter path.
    Syntax: "\object(instance)\counter" or "\object\counter".
    See examples in "CPerformanceCounter help".
    Get the strings from the Add Counters dialog of Performance Monitor (perfmon.exe).
    Wildcard characters can be used in the instance part.


Close
int e
e=PdhOpenQueryW(0 0 &m_hq); if(e) goto ge
e=PdhAddCounterW(m_hq @counter 0 &m_hc); if(e) PdhCloseQuery(m_hq); m_hq=0; goto ge
if(findcs(counter "*?")>=0) m_flags|1
ret
 ge
end _s.dllerror("" "pdh" e)
