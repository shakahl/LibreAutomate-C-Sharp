 Create shortcut with all possible attributes
SHORTCUTINFO si.target="$windows$\notepad.exe"
si.iconfile="shell32.dll"
si.iconindex=4
si.descr="test descr"
si.hotkey=HOTKEYF_CONTROL<<8|VK_F6
si.initdir="$my qm$"
si.param="-p"
si.showstate=SW_MAXIMIZE

out CreateShortcutEx("$desktop$\test.lnk" &si)
