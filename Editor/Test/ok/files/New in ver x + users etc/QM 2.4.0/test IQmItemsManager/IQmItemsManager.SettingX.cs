 out
lpstr macro=+qmitem
str sb="BINARY"

PF
_qmfile.SettingAddB(macro "bin" sb sb.len)
_qmfile.SettingAddS(macro "str" "STRING")
_qmfile.SettingAddI(macro "int" 1000000)
_qmfile.SettingAddB(macro "_nul" 0 0)
str s1 s2
PN
s1.all(sb.len 2); rep(2) _qmfile.SettingGetB(macro "bin" s1 s1.len); PN
 out _qmfile.SettingGetS(macro "str"); PN
rep(2) _qmfile.SettingGetS(macro "str" s2); PN
rep(2) _i=_qmfile.SettingGetI(macro "int"); PN
 _i=-100; _i=_qmfile.SettingGetI(macro "no"); err _i=8
lpstr n=_qmfile.SettingGetS(macro "_nul")
PO
out s1
out s2
out _i
out "%s %i" n n

 _qmfile.SettingDelete(macro "bin")
 _qmfile.SettingDelete(macro "str")
 _qmfile.SettingDelete(macro "int")
 _qmfile.SettingDelete(macro "_nul")
