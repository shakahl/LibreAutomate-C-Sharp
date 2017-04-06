 /CtoQM
function IStringMap&m2 !Type ;;Type: 1 def, 2 guid, 3 dll, 4 type, 5 iface, 6 typedef

 Appends m2 to m_mall.
 Prepends def NAME to constants and GUIDs.
 Converts C escape sequences in strings.
 Calls CalcConst to simplify constants.
 Appends duplicates as comments.

str k v s ss
m2.EnumBegin
rep
	if(!m2.EnumNext(k v)) break
	
	sel Type
		case 1
		if(CalcConst(k v)) continue
		s.format("def %s %s" k v)
		if(findc(s 34)>=0) if(!EscapeCString(&s 3)) end s 1
		
		case 2
		s.format("def %s %s" k v)
		
		case 6
		s.format("type %s = %s" k v)
		if(m_mfcb.Get2(k ss)) s.formata("[] ;;%s" ss)
		AddToMap(m_mo k s "" 1)
		continue
		
		case else s=v
	
	lpstr comm=m_mcomm.Get(k); if(comm) s+"[]"; s+comm;; out s
	m_mall.Add(k s)
	err
		ss.format("%s[] ;;%s" m_mall.Get(k) s)
		 out ss
		m_mall.Set(k ss)
		
