out
str s0 s1 s2 s4
word* w
 ITEMIDLIST* pidl=PidlFromStr("$pf$\quick macros 2\qm.exe") ;;0x40400177 
ITEMIDLIST* pidl=PidlFromStr("$17$") ;;0xb0000174 (SFGAO_HASSUBFOLDER|SFGAO_FOLDER|SFGAO_FILESYSANCESTOR|SFGAO_DROPTARGET|SFGAO_HASPROPSHEET|SFGAO_CANDELETE|SFGAO_CANRENAME|SFGAO_CANLINK)
 ITEMIDLIST* pidl=PidlFromStr("::{20D04FE0-3AEA-1069-A2D8-08002B30309D}")
 ITEMIDLIST* pidl=PidlFromStr("http://www.quickmacros.com") ;;0x8400004 (SFGAO_BROWSABLE|SFGAO_STREAM|SFGAO_CANLINK)
 ITEMIDLIST* pidl=PidlFromStr("mailto:support@quickmacros.com")

int f=SIGDN_DESKTOPABSOLUTEPARSING
PF
rep 1
	BSTR b.alloc(300)
	if(SHGetPathFromIDListW(pidl b.pstr)) s0=b
PN
rep 1
	if(!SHGetNameFromIDList(pidl f &w))
		s1.ansi(w)
		CoTaskMemFree w
PN
rep 1
	IShellItem si
	if(SHCreateShellItem(0 0 pidl &si)) ret
	si.GetDisplayName(f &w); err continue
	si=0
	s2.ansi(w)
	CoTaskMemFree w

PN
rep 1
	 PidlToStr(pidl s4)
	PidlToStr(pidl s4 16)
PN
PO

out s0
out s1
out s2
out "---"
out s4
