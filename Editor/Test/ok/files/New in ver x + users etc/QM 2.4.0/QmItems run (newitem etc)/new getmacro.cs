out
dll "qm.exe" !TestGetmacro id str*s [mask]
1
str s
PF
rep 3
	 _i=TestGetmacro(1 &s)
	 _i=TestGetmacro("test3" &s 2)
	 _i=TestGetmacro("testR" &s 3)
	 _i=TestGetmacro("testR" &s 6)
	 _i=TestGetmacro("testR" &s 7)
	_i=TestGetmacro("init" &s 8)
	PN
PO
out _i
out s

#ret
_s.getmacro
 getmacro([$macro|iid] [flags])   ;;`Gets macro text, name, etc.`
 flags: 0 text, 1 name, 2 trigger,
   3 type ("0" macro, "1" function, "2" menu, "3" toolbar, "4" autotext, "5" folder, "6" member, "7" file link),
   4 disabled, 5 encrypted, 6 shared, 7 id, 8 read-only.
