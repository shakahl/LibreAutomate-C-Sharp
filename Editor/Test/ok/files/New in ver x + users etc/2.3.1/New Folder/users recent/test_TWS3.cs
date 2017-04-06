str vbs=
 set t=CreateObject("TWS.TwsCtrl.1")
 t.connect "", 7496, 0
 msgbox 1

vbs.setfile("$temp$\tws.vbs")

run "$temp$\tws.vbs"
