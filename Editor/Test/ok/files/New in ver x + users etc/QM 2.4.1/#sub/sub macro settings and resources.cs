_qmfile.SettingAddI(+-1 "test" 3)
out _qmfile.SettingGetI(+-1 "test")
 _qmfile.ResourceAdd(+-1 "test" "par" 3)
 _qmfile.ResourceGet(+-1 "test" _s); out _s
sub.Test

#sub Test
_qmfile.SettingAddI(+-1 "test" 4)
out _qmfile.SettingGetI(+-1 "test")
 _qmfile.ResourceAdd(+-1 "test" "sub" 3)
 _qmfile.ResourceGet(+-1 "test" _s); out _s
sub.Test2

#sub Test2
_qmfile.SettingAddI(+-2 "test" 5)
 _qmfile.SettingAddI("sub macro settings and resources" "test" 5)
 _i=getopt(itemid); out _i; _qmfile.SettingAddI(+_i "test" 5)
 _s=getopt(itemname); out _s; _qmfile.SettingAddI(_s "test" 5)
out _qmfile.SettingGetI(+-1 "test")
 _qmfile.ResourceAdd(+-1 "test" "sub" 3)
 _qmfile.ResourceGet(+-1 "test" _s); out _s
