\Dialog_Editor

_monitor=2 ;;must be ignored

if(!ShowDialog("ds_centermouse" 0)) ret
 if(!ShowDialog("ds_centermouse" 0 0 _hwndqm)) ret
 if(!ShowDialog("ds_centermouse" 0 0 0 0 0 0 0 1 1)) ret ;;test in all corners of all monitors
if(!ShowDialog("ds_centermouse" 0 0 _hwndqm 0 0 0 0 -100 0)) ret ;;test in all corners of all monitors
 if(!ShowDialog("ds_centermouse" 0 0 0 0 0 0 0 -100 100)) ret

 BEGIN DIALOG
 0 "" 0x90C81244 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020104 "" "" ""
