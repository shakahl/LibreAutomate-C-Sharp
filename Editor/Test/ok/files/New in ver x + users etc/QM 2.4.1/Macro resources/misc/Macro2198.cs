out
ChDir "$my qm$"
str ss=
 Macro2198.bmp
 Macro2198-8.bmp
 Macro2198 (3).bmp
 Macro2198 (3)-8.bmp
 Macro2198 (4).bmp
 Macro2198 (4)-8.bmp
 Macro2198 (5).bmp
 Macro2198 (5)-8.bmp
str s
foreach s ss
	out s
	_s.getfile(s)
	out "bmp: %i" _s.len
	_s.encrypt(32)
	out "lzo: %i" _s.len
