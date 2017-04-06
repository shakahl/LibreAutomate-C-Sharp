out
int w=win("Options" "#32770")
Acc a.Find(w "CHECKBUTTON" "Tray icon" "class=Button[]id=1098" 0x1005)
BSTR v d
v=a.a.Value
out _hresult
d=a.a.Description
out _hresult
out v
out d
out "%i %i" v.pstr d.pstr
out a.a.KeyboardShortcut
out _hresult
VARIANT k.vt=VT_I4
out a.a.Help(k)
out _hresult
BSTR bh
out a.a.HelpTopic(&bh k)
out _hresult
out bh
a.a.Value(k)="fff"
out _hresult
