;; Information about automation:
;; If you want to have a programable task (or just a shortcut) for backup,
;; you need to run this macro like it:
;; qm.exe M "SB_BackUp" "X ABCDEF"
;; Where X is a number between 1 and 3 and ABCDEF is the path to the config file.
;; If X is 1, it runs only backup.
;; If X is 2, runs maintenance (deletion of removed files and old versions)
;; If X is 3 runs both.

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 208 124 "Dialog"
 4 Button 0x54032000 0x0 75 70 50 20 "Salir"
 2 Button 0x54032000 0x0 120 10 60 30 "Limpiar backup"
 1 Button 0x54032000 0x0 20 10 60 30 "Backup"
 7 Edit 0x54030080 0x200 6 104 136 14 ""
 5 Button 0x54032000 0x0 150 104 48 14 "Browse"
 6 Static 0x54000000 0x0 8 94 56 14 "Configuration file"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""

str IniFile = "C:\Backup.ini" ;; Default file.
str Log
str OrigFolder DestFolder
str Versions ;; Values for max.versions and maintenance
str+ reg=""

int i1 i2 switch
str s1 s2
ARRAY(str) Ext Files
ARRAY(str) Atr
switch = 4
if(_command)
	switch = val(getTok(_command 0 -1 " "))
	INIFile = getTok(_command 1 2 " " 2)
if (switch>3 || switch<0)
	str controls = "7"
	str e7 = INIFile
	 MENU
	switch = ShowDialog("SB_BackUp" 0 &controls)

sel switch
	case [1,3]:
		i1 = 0 ;;ConfigX, where X = i1
		rep
			i1 = i1 + 1
			s1.format("Config%i" i1)
			if (rget(OrigFolder "Orig" s1 IniFile))
				rget DestFolder "Dest" s1 IniFile
				rget Versions "File0" s1 IniFile
				i2 = 0
				Ext.create
				Atr.create
				rep
					i2 = i2 + 1 ;;FileX, where X = i2
					s2.format("File%i" i2)
					if (rget(_s s2 s1 IniFile))
						Ext[] = getTok(_s 0 -1 ",")
						Atr[] = getTok(_s 1 2 "," 2)
						err
							Atr[Atr.len-1] = "10"
					else break;
				Ext[] = "..."
				Atr[] = Versions
				SB_Search(Files OrigFolder DestFolder)
				for _i 0 Files.len
					_s.GetFilenameExt(Files[_i])
					SB_Copy(Files[_i] OrigFolder DestFolder SB_getAttribute(Atr[SB_getIDExtension(&Ext _s)] 0))
			else break
		if(switch = 3)goto STEP2

	case [2,0]:
		 STEP2
		reg.formata("%s: MAINTENANCE[]" _s.time) ;; It's included, because log doesn't record versions deletions (Personal prefer).
		i1 = 0 ;;ConfigX, where X = i1
		rep
			i1 = i1 + 1
			s1.format("Config%i" i1)
			if (rget(OrigFolder "Orig" s1 IniFile))
				rget DestFolder "Dest" s1 IniFile
				rget Versions "File0" s1 IniFile
				i2 = 0
				Ext.create
				Atr.create
				rep
					i2 = i2 + 1 ;;FileX, where X = i2
					s2.format("File%i" i2)
					if (rget(_s s2 s1 IniFile))
						Ext[] = getTok(_s 0 -1 ",")
						Atr[] = getTok(_s 1 2 "," 2)
						err
							Atr[Atr.len-1] = "10"
					else break;
				Ext[] = "..."
				Atr[] = Versions
				SB_SearchInv(Files OrigFolder DestFolder)
				SB_Clean(Files DestFolder Ext Atr)
			else break

	case 4:
		ret

	case 5:
		if (OpenSaveDialog(0 _s "INI Files[]*.ini"))
			INIFile = _s;e7 = _s
		goto MENU
if (reg.len > 0) ;; Recording to Registry
	if (rget(Log "Logfile" "Global" IniFile))
		s1.getfile(Log)
		err ;; If Log doesn't exist, app don't crash.
		_s.format("%s%s" s1 reg)
		_s.setfile(Log)
out "END"
