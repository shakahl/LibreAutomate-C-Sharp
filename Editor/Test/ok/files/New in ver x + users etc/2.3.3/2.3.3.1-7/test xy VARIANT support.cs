VARIANT vx=0.5
VARIANT vy=10

 mou 100 300
 mou 0.1 0.5
 mou vx vy
 mou 100 10 "" 1
 mou 0.1 0.5 ""
 mou vx vy "" 1

 rig 100 300
 rig 0.1 0.5
 rig vx vy

 outx pixel(808 117)

 double dx(0.02) dy(0.9)
  mou dx dy
  outx pixel(dx dy)
 mou dx dy ""
 outx pixel(dx dy "")

 mov 10 100
 mov 0.5 0.1
 mov vx vy ""
 mov vx vy "" 1

 wait 0 C 0xB2F7FF 23 538
 wait 0 C 0x3EB486 40 256 win("Pagrindinė paieška - Pažintys - Draugas.lt - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
 wait 0 C 0x41B588 33 225 win("Pagrindinė paieška - Pažintys - Draugas.lt - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804) 1

 int w=win("QM TOOLBAR" "QM_toolbar")
  Acc a=acc(0.5 10 w)
  Acc a=acc(vx vy w)
  vx=115; vy=0.0
  Acc a=acc("" "PUSHBUTTON" w "" "" 0x1100 vx vy)
 out a.Name

 outw child(20 100)
 outw child(20 100 0)
 outw child(20 100 _hwndqm)
 outw child(20 100 _hwndqm 32)
 outw child(0.5 0.5)
 outw child(vx vy)
 outw child("" "" "" 0 vx 50)

 int w=win("QM TOOLBAR" "QM_toolbar")
  outw child("" "ToolbarWindow32" w)
 outw child(9999 "" "ToolbarWindow32" w)

 outw win(10 100)
 outw win(10 100 1)
 outw win(0.5 0.5)
 outw win(vx vy)
 outw win("" "" "" 0 vx vy)

