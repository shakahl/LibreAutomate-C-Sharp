int w=win("TOOLBAR1" "QM_toolbar"); if(w) clo w

_qmfile.SettingDelete("Toolbar1" "tb") ;;reset Toolbar1 in default layout
 or
_qmfile.SettingDelete("Toolbar1" "tb Layout Name") ;;reset Toolbar1 in another layout
 or
_qmfile.SettingDelete("Toolbar1" "tb*") ;;reset Toolbar1 in all layouts
