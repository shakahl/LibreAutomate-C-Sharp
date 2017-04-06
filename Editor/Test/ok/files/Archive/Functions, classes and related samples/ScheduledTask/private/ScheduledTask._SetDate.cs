function TASK_TRIGGER&t $_date [flags] ;;flags: 1 end date

DATE d; SYSTEMTIME st
d=_date; err end "invalid date string"
d.tosystemtime(st)
word* p=iif(flags&1 &t.wEndYear &t.wBeginYear)
p[0]=st.wYear
p[1]=st.wMonth
p[2]=st.wDay
