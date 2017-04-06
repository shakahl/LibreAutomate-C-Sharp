if(child!=SHFB_FindEditorControl) key T; ret
spe 1
key CSR
str s.getsel
 out s
sel s
	case "</"
	key CSR SR
	s.getsel
	s.get(s 2 s.len-3)
	 out s
	sel s
		case ["para","alert","quote"]
		key E Y
		SHFB_insert "<para>##</para>" 1
		
		case else
		key Z
	
	case "[]"
	key L
	SHFB_insert "<para>##</para>" 1
	
	case "<"
	key LT
	
	case else
	key T
	