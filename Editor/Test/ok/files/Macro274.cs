str s
s="$common programs$\Microsoft Office\Microsoft Office Access 2003.lnk"
s="$common programs$\VideoLAN\VLC media player.lnk"
s="$desktop$\ClassicStartMenu.exe - Shortcut.lnk"
 s="$programs$\Test\virtual.lnk"
 s="$programs$\Test\URL.lnk"

Wsh.WshShell sh._create
Wsh.WshShortcut x=+sh.CreateShortcut(_s.expandpath(s))
out x.TargetPath
out x.IconLocation


#ret
HOTKEYF_ALT
ALT key
HOTKEYF_CONTROL
CTRL key
HOTKEYF_EXT
Extended key
HOTKEYF_SHIFT
