dll- "qmjab.dll"
	#JabTest x
	IAccessible'JabFromWindow hwnd [flags] ;;flags: 1 focus
	IAccessible'JabFromPoint POINT'p [hwnd]
	IAccessible'JabAccFromAC vmID ac
	JabShutdown
