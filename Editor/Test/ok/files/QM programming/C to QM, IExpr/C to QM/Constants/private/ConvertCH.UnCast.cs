 /CtoQM
function# str&s [dontleave]

 Removes all type casts, eg (DWORD)a -> a.
 If dontleave is 0, leaves typecasts where removing is unsafe. If 1, removes all.
 Removes only what is found in m_mtd, but leaves structs and unknown.
 Returns 1 if some are leaved.

int i removed leaved db; str ss sss
rep
	i=findrx(s m_rx.cast i 0 ss 1); if(i<0) break ;;find (CASTTYPE)
	if(ss.end("*")) goto rem
	if(m_mtd.Get2(ss sss))
		if(sss.end("*")) goto rem
		sel(sss[0])
			case ['#','$','%'] goto rem
			case else if(dontleave) goto rem; else leaved=1
			 It is unsafe to remove (WORD), (BYTE), etc. Examples: (WORD)-1 is 0xffff, (SHORT)0xffff is negative. Removes and adds comments.
		 out sss
	 db=1
	i+ss.len; continue
	 rem
	i-1; s.remove(i ss.len+2); removed=1
	 db=1
if(removed) UnPar(s)
ret leaved
