sel mes("Run QM at startup" "" "YN?")
	case 'Y'
	str s.format("''%sqm.exe'' S" _qmdir)
	rset s "Quick Macros" "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" HKEY_LOCAL_MACHINE
	case 'N'
	rset "" "Quick Macros" "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" HKEY_LOCAL_MACHINE -1
	