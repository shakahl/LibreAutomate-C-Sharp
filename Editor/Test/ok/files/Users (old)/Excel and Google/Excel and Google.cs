act " Internet Explorer"
ARRAY(str) a
foreach a "" FE_ExcelRow
	Htm el=htm("INPUT" "as_q" "" " Internet Explorer" 0 0 0x221 5)
	el.SetText(a[0])
	el=htm("INPUT" "as_epq" "" " Internet Explorer" 0 3 0x221)
	el.SetText(a[1])
	el=htm("INPUT" "as_oq" "" " Internet Explorer" 0 4 0x221)
	el.SetText(a[2])
	el=htm("INPUT" "as_eq" "" " Internet Explorer" 0 5 0x221)
	el.SetText(a[3])
	1
	 el=htm("INPUT" "Google Search" "" " Internet Explorer" 0 2 0x421)
	 el.Click
	 5
	 web "Back"
