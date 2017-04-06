 /
function hwnd [$prefix] [str&sOut]

 Displays window handle, class and text (first line) in QM output.

 sOut (QM 2.3.5) - if used, stores text in the variable, else displays in QM output.

 Added in: QM 2.3.2.


str s
if(!hwnd) s.format("%s0" prefix)
else if(!IsWindow(hwnd)) s.format("%s%i <invalid handle>" prefix hwnd)
else
	str sc.getwinclass(hwnd); err
	str st.getwintext(hwnd); err
	st.trim; st.getl(st 0)
	s.format("%s%i %s ''%s''" prefix hwnd sc st)
if(&sOut) sOut=s; else out s
