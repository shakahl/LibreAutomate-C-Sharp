function $Command [~Target] [~Value]

opt noerrorshere 1
_Post(F"{Command}&1={Target.escape(9)}&2={Value.escape(9)}")
 str sr
 _Post(F"{Command}&1={Target.escape(9)}&2={Value.escape(9)}" sr)

 workaround hidden FF window
if m_ffNeedWorkaround and !StrCompare(Command "open")
	m_ffNeedWorkaround=0
	int w1=win("Selenium Remote Control* - Mozilla Firefox" "MozillaWindowClass" "" 0x5)
	int pid; GetWindowThreadProcessId(w1 &pid)
	ARRAY(int) a; int i
	win "* - Mozilla Firefox" "MozillaWindowClass" pid 0x1 "" a
	for i 0 a.len
		int w=a[i]
		 outw w
		if(w!w1 and hid(w)) hid- w; err
