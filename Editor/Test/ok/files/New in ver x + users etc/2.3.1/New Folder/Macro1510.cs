Acc a=acc("" "STATUSBAR" win("WordPad" "WordPadClass") "msctls_statusbar32" "" 0x1000)
for a.elem 1 1000
	str s=a.Name; err break
	out s
