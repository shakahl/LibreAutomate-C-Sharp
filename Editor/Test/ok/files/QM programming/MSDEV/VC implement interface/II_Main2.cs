mes "Select interface member functions"
str s.getsel sdecl sdef cls
mes "Select class name"
cls.getsel
II_Func &s &cls &sdecl &sdef 0
mes "Where to insert declarations"
sdecl.setsel
mes "Where to insert definitions"
sdef.setsel
