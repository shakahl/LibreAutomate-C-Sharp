function# [int&nInstances]

 Returns current value of the counter.
 On failure returns 0, does not throw error.

 nInstances - if used, receives number of matching instances when wildcard characters used.


def PDH_MORE_DATA 0x800007D2

if(&nInstances) nInstances=0
if(!m_hq) ret
if(PdhCollectQueryData(m_hq)) ret

if !(m_flags&1) ;;single
	PDH_FMT_COUNTERVALUE ff
	if(PdhGetFormattedCounterValue(m_hc PDH_FMT_LONG 0 &ff)) ret
	ret ff.longValue
else ;;multiple
	int i j r
	if(PdhGetFormattedCounterArrayW(m_hc PDH_FMT_LONG &i &j 0)!=PDH_MORE_DATA) ret
	ARRAY(PDH_FMT_COUNTERVALUE_ITEM_W) a.create(i/sizeof(PDH_FMT_COUNTERVALUE_ITEM_W)+1) ;;structs are followed by strings
	if(PdhGetFormattedCounterArrayW(m_hc PDH_FMT_LONG &i &j &a[0])) ret
	for i 0 j
		 out F"{a[i].szName%%S}"
		if(!wcscmp(a[i].szName L"_Total")) continue
		r+a[i].FmtValue.longValue
	
	if(&nInstances) nInstances=j
	ret r
