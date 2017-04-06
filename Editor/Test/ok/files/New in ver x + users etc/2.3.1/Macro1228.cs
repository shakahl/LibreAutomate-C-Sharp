out
 out _s.timeformat
 out _s.timeformat("")
 out _s.timeformat("{DD} {TT}")
 out _s.timeformat("{DD} {TT}" 0)
 out _s.timeformat("{DD} {TT}" 0 WINAPI.LANG_RUSSIAN)
 out _s.timeformat("{DD} {TT}" 0 WINAPI.LANG_RUSSIAN 8)
 out _s.timeformat("{DD} {TT}" 0 WINAPI.LANG_RUSSIAN 8 2)


DATE d; SYSTEMTIME st; FILETIME ft
d.getclock; d=d+10
 d.date=-100000000000
 d.date+10000000000
d.tosystemtime(st)
 st.wYear=1601; st.wMonth=1; st.wDay=1
d.tofiletime(ft)
out _s.timeformat("{DD} {TT}" d)
out _s.timeformat("{DD} {TT}" st)
out _s.timeformat("{DD} {TT}" ft)

 out _s.timeformat("{DD} {TT}" &d)
 DATE& dr=d; out _s.timeformat("{DD} {TT}" dr)


 RECT r
 DATE* r
 _s.timeformat("{DD} {TT}" &r)
 _s.timeformat("{DD} {TT}" "fff")
 _s.timeformat("{DD} {TT}" _i)
