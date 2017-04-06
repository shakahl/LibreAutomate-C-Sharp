 /
function $folder [flags] ;;flags: 1 include subfolders, 2 only qm shortuts, 4 only with hotkeys

 Displays parameters (target, hotkey, etc) of all shortcuts in the folder.

 EXAMPLES
  QM shortcuts on desktop
 EnumShortcutsInFolder "$desktop$" 2
 EnumShortcutsInFolder "$common desktop$" 2
  all shortcuts with hotkeys in start menu and submenus
 EnumShortcutsInFolder "$start menu$" 1|4
 EnumShortcutsInFolder "$common start menu$" 1|4


str s sf.from(folder "\*.lnk") sq.from(_qmdir "qm*.exe") hk
Dir d
foreach(d sf FE_Dir iif(flags&1 0x4 0))
	str sPath=d.FileName(1)
	SHORTCUTINFO si
	if(!GetShortcutInfoEx(sPath &si)) continue
	if(flags&2 and !matchw(si.target sq 1)) continue
	if(flags&4 and !si.hotkey) continue
	out sPath
	s.getstruct(si 1)
	hk=""; FormatKeyString si.hotkey&255 si.hotkey>>8&7 &hk; s.replacerx("(?m)(?<=^hotkey )\w+" hk)
	s.replacerx("(?m)^" "[9]")
	out s
