function# param str&a str&b

ret StrCompareLogical(a b)


 dll- shlwapi #StrCmpLogicalW @*psz1 @*psz2
 
 if(_winver>=0x501) ;;XP
	 BSTR w1(a) w2(b)
	 ret StrCmpLogicalW(w1 w2)
 else
	  ret StrCompare(a b 1)
	 ret StrCompareLogical(a b)
