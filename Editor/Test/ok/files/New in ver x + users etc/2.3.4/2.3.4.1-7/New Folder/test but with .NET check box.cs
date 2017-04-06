 int w=win("Keyboard Layout Creator 1.4 - 'Layout01 Description'" "WindowsForms10.Window.8.app.0.33c0d9d")
 int c=child("Show the Caps Lock" "WindowsForms10.BUTTON.app.0.33c0d9d" w)

 int w=win("Options" "#32770")
 int c=id(1010 w)

 int w=win("Calculator" "CalcFrame")
 int c=id(110 w)

int w=win("Form1" "WindowsForms10.Window.8.app3")
 int c=child("normal" "WindowsForms10.BUTTON.app3" w)
int c=child("3state" "WindowsForms10.BUTTON.app3" w)


 out but(c)
 act w
 but c
 but+ c; 0.5; but- c
 but+ c
 but- c
 but* c
 but% c


 int msg=RegisterWindowMessage("WM_GETCONTROLNAME")
 Q &q
  SendMessageTimeout(c msg 0 0 SMTO_ABORTIFHUNG 10000 &_i)
  _i=but(c)
  _s.getwinclass(c)
 GetWindowThreadProcessId(c 0)
 Q &qq
 outq
 
 out _i


 Acc a.FromWindow(c OBJID_CLIENT)
 a.Role(_s)
 out _s
