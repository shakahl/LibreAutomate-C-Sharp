str s=FirefoxGetAddress
 out s
s.replacerx("^.+\bv=(.+?)(?=&|$)" "$1" 4)
 out s
#region Recorded 2017-02-05 23:02:49
int w1=act(win("YouTube - Mozilla Firefox" "MozillaWindowClass"))
'SCb            ;; Shift+Ctrl+B
int w2=wait(16 win("Library" "MozillaWindowClass"))
'Cf             ;; Ctrl+F
s.setsel
#endregion
