 /
function [DATE'date]

int+ g_datediff
DATE dnow.getclock
SYSTEMTIME st

if(!g_datediff) ;;change
	if(!getopt(nargs)) ret
	 remove hours
	int i j
	i=dnow.date; dnow=i
	j=date.date; date=j
	
	g_datediff=i-j
	if(!g_datediff) ret
	date.tosystemtime(st)
	out "Date changed to %s" _s.from(date)
else ;;restore
	dnow=dnow+g_datediff
	g_datediff=0
	dnow.tosystemtime(st)
	out "Date restored to %s" _s.from(dnow)

Date st.wYear st.wMonth st.wDay
		