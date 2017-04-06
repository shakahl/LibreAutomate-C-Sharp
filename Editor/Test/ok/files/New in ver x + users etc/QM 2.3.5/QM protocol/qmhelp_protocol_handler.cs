str s=_command+7
s.escape(8) ;;sometimes command is urlencoded, eg from firefox
 out s
0.1 ;;prevent IE flashing taskbar button
act _hwndqm
QmHelp s 0 4
