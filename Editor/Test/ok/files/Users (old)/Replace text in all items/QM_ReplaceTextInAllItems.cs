spe 10
int hwnd=win("Find" "#32770" "QM")
int hlb=id(1128 hwnd)
int hb=id(1132 hwnd)
act hlb
key H
rep LB_GetCount(hlb)
	key+ C
	but hb
	key- C
	act hlb
	key DV
