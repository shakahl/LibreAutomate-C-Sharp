 \Dialog_Editor
function# hDlg message wParam lParam
str s s1 s2 IniFile pattern
int i1 i2
int hlbC = id(1119 hDlg)
int hlbF = id(1120 hDlg)
int- ControlModify = 1
int- Config
int- FExtension
Dir d
ARRAY(str)- TempArray

if(hDlg) goto messages

 Example of multipage dialog calling code: 
 str controls = "3"
 str lb3
 lb3="&Page0[]Page1[]Page2"
 if(!ShowDialog("ThisFunction" &ThisFunction &controls)) ret
 ...

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 471 279 "Dialog"
 90 Static 0x54000000 0x0 12 24 48 12 "Config file"
 1180 Static 0x54000002 0x0 126 208 48 13 "Delete files"
 1181 Static 0x54000000 0x0 128 104 48 12 "Source"
 1182 Static 0x54000000 0x0 126 128 48 13 "Destination"
 1183 Static 0x54000002 0x0 134 162 40 10 "Versions"
 1184 Static 0x54000002 0x0 122 184 52 16 "Delete versions"
 1185 Static 0x54000000 0x0 12 102 80 10 "Configurations available"
 1186 Static 0x54000000 0x0 396 154 48 12 "File extension"
 1187 Static 0x54000000 0x0 396 176 48 12 "Versions"
 1188 Static 0x54000000 0x0 396 194 54 10 "Delete Versions"
 1189 Static 0x54000000 0x0 396 214 48 12 "Delete Files"
 1190 Static 0x54000000 0x0 12 50 48 13 "LogFile"
 1109 Edit 0x54020880 0x200 180 104 218 14 ""
 1 Button 0x5C030001 0x4 302 254 48 14 "Save"
 2 Button 0x54030000 0x4 364 254 48 14 "Cancel"
 3 Button 0x54032000 0x4 420 254 18 14 "?"
 4 Edit 0x54030880 0x200 66 22 310 14 ""
 25 Button 0x54032000 0x0 382 22 50 16 "Browse"
 1106 Button 0x54012003 0x0 68 50 10 12 "chkLog"
 1105 Edit 0x54030080 0x200 82 50 294 14 ""
 1126 Button 0x54032000 0x0 382 50 50 16 "Browse"
 1119 ListBox 0x54230101 0x200 10 116 80 94 ""
 1127 Button 0x54032000 0x0 406 104 48 16 "Browse"
 1110 Edit 0x54030080 0x200 180 126 218 16 ""
 1128 Button 0x54032000 0x0 406 126 48 16 "Browse"
 1111 Edit 0x5C032081 0x300 182 158 28 16 ""
 1112 Edit 0x5C030081 0x200 182 182 28 16 ""
 1113 Edit 0x5C030081 0x200 182 206 28 16 ""
 1120 ListBox 0x54230101 0x200 240 160 96 48 ""
 1114 Edit 0x5C030081 0x200 354 152 36 16 ""
 1115 Edit 0x5C030081 0x200 354 173 28 16 ""
 1116 Edit 0x5C030081 0x200 354 192 28 16 ""
 1117 Edit 0x5C030081 0x200 354 212 28 16 ""
 1121 Button 0x54022000 0x0 18 218 20 15 "+"
 1122 Button 0x54022000 0x0 58 218 20 15 "-"
 1123 Button 0x54022000 0x0 254 218 20 14 "+"
 1124 Button 0x54022000 0x0 296 218 20 14 "-"
 131 Button 0x54020007 0x0 8 6 452 68 "Step 1"
 1132 Button 0x54020007 0x0 4 86 106 154 "Step 2"
 1133 Button 0x54020007 0x0 118 86 342 154 "Step 3"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "1" ""
ret
 messages
sel message
	case WM_INITDIALOG
		__Font-- f
		f.Create("Courier New" 18 1)
		f.SetDialogFont(hDlg "131 1132 1133")
		f.SetDialogFontColor(hDlg 0xff0000 "131 1132 1133")
		DT_Page hDlg 0
		but+ id(1106 hDlg)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 25 ;; Browse INI File
		if OpenSaveDialog(1 s "Config files[]*.ini")
			INIFile = s
			Config = -1
			FExtension = -1
			Sources.create(0)
			Configs.create(0)
			DT_Page hDlg 1
			EnableWindow id(1 hDlg) 1 
			for _i 1109 1118
				EnableWindow id(_i hDlg) 0
			SendMessage hlbC LB_RESETCONTENT 0 0
			SendMessage hlbF LB_RESETCONTENT 0 0
			s.setwintext(id(4 hDlg))
			IniFile = s
			if (d.dir(s))
				if (rget(Log "Logfile" "Global" IniFile))
					Log.setwintext(id(1105 hDlg))
				else
					but- id(1106 hDlg)
				i1 = 0 ;;ConfigX, where X = i1
				rep
					i1 = i1 + 1
					s1.format("Config%i" i1)
					if (rget(_s "Orig" s1 IniFile))
						rget s "Dest" s1 IniFile
						_s.formata("[]%s" s)
						Sources[] = _s
						LB_Add(hlbC s1 0)
						rget _s "File0" s1 IniFile
						i2 = 0
						rep
							i2 = i2 + 1 ;;FileX, where X = i2
							s2.format("File%i" i2)
							if (rget(s s2 s1 IniFile))
								_s.formata("[]%s" s)
							else
								Configs[] = _s
								break;
					else break

	case 1126 ;; LogFile
		if OpenSaveDialog(1 s "Log files[]*.log")
			s.setwintext(id(1105 hDlg))
			if (but(id(1106 hDlg)))
				Log = s

	case 1106 ;;Change logfile
		if (but(id(1106 hDlg)))
			Log.getwintext(id(1105 hDlg))
		else
			Log = ""

	case EN_KILLFOCUS<<16|1105 ;; Enable/Disable changelog checkbox
		if (but(id(1106 hDlg)))
			Log.getwintext(id(1105 hDlg))
		else
			Log = ""

	case LBN_SELCHANGE<<16|1119 ;; Change Config
		ControlModify = 0
		_i=LB_SelectedItem(lParam)
		TempArray = Sources[_i]
		TempArray[0].setwintext(id(1109 hDlg))
		TempArray[1].setwintext(id(1110 hDlg))
		TempArray = Configs[_i]
		Config = _i
		_s = getTok(TempArray[0] 0 -1 ",")
		_s.setwintext(id(1111 hDlg))
		_s = getTok(TempArray[0] 1 -1 ",")
		_s.setwintext(id(1112 hDlg))
		_s = getTok(TempArray[0] 2 -1 ",")
		_s.setwintext(id(1113 hDlg))
		SendMessage hlbF LB_RESETCONTENT 0 0
		_s = ""
		for i1 1114 1118
			_s.setwintext(id(i1 hDlg))
		for i1 1 TempArray.len
			s1.format("%s" getTok(TempArray[i1] 0 -1 ","))
			s1.ucase(s1)
			_s.format("Filetype %s" s1)
			LB_Add(hlbF _s 0)
		for _i 1109 1114
			EnableWindow id(_i hDlg) 1
		for _i 1114 1118
			EnableWindow id(_i hDlg) 0
		ControlModify = 1

	case LBN_SELCHANGE<<16|1120 ;; Select Extension
		ControlModify = 0
		_i=LB_SelectedItem(lParam)
		FExtension = _i + 1
		TempArray = Configs[Config]
		_s = getTok(TempArray[_i+1] 0 -1 ",")
		err _s = ""
		_s.ucase(_s)
		_s.setwintext(id(1114 hDlg))
		_s = getTok(TempArray[_i+1] 1 -1 ",")
		err _s = "-1"
		_s.setwintext(id(1115 hDlg))
		_s = getTok(TempArray[_i+1] 2 -1 ",")
		err _s = "-1"
		_s.setwintext(id(1116 hDlg))
		_s = getTok(TempArray[_i+1] 3 -1 ",")
		err _s = "-1"
		_s.setwintext(id(1117 hDlg))
		for _i 1114 1118
			EnableWindow id(_i hDlg) 1
		ControlModify = 1

	case 1127 ;;Browse Source
		if(BrowseForFolder(s))
			s.setwintext(id(1109 hDlg))
			TempArray = Sources[Config]
			TempArray[0] = s
			Sources[Config] = TempArray

	case 1128 ;;Browse Destination
		if(BrowseForFolder(s))
			s.setwintext(id(1110 hDlg))
			TempArray = Sources[Config]
			TempArray[1] = s
			Sources[Config] = TempArray

	case EN_KILLFOCUS<<16|1110 ;; Changed Destination
		if (ControlModify)
			s.getwintext(id(1110 hDlg))
			TempArray = Sources[Config]
			TempArray[1] = s
			Sources[Config] = TempArray

	case EN_CHANGE<<16|1111 ;;Change global V
		if (ControlModify)
			if (Config < 0)
				_s = ""
				_s.setwintext(id(1111 hDlg))
			else
				_s.getwintext(id(1111 hDlg))
				if ((val(_s) > 99) || val(_s) < 0)
					TempArray = Configs[Config]
					_s = getTok(TempArray[0] 0 -1 ",")
					_s.setwintext(id(1111 hDlg))
				else
					TempArray.create(0)
					TempArray = Configs[Config]
					_s.format("%s," s1.getwintext(id(1111 hDlg)))
					if (_s.len > 1)
						_s.formata("%s," getTok(TempArray[0] 1 -1 ","))
						err	_s.formata("-1,")
						_s.formata("%s" getTok(TempArray[0] 2 -1 ","))
						err	_s.formata("-1")
						TempArray[0] = _s
						Configs[Config] = TempArray
	
	case EN_CHANGE<<16|1112 ;;Change Global VD
		if (ControlModify)
			if (Config < 0)
				_s = ""
				_s.setwintext(id(1112 hDlg))
			else
				_s.getwintext(id(1112 hDlg))
				pattern = "(^$)|(^\-1$)|(^-$)|(^([0-9]){0,4}$)"
				if(findrx(_s pattern 0) < 0)
					TempArray = Configs[Config]
					_s = getTok(TempArray[0] 1 -1 ",")
					_s.setwintext(id(1112 hDlg))
				else
					TempArray.create(0)
					TempArray = Configs[Config]
					s1.getwintext(id(1112 hDlg))
					if (_s.len > 1)
						_s.format("%s," getTok(TempArray[0] 0 -1 ","))
						err	_s.format("10")
						_s.formata("%s," s1)
						_s.formata("%s" getTok(TempArray[0] 2 -1 ","))
						err	_s.formata("-1")
						TempArray[0] = _s
						Configs[Config] = TempArray
	
	case EN_CHANGE<<16|1113 ;;Change Global FD
		if (ControlModify)
			if (Config < 0)
				_s = "";_s.setwintext(id(1113 hDlg))
			else
				_s.getwintext(id(1113 hDlg))
				pattern = "(^\-1$)|(^-$)|(^([0-9]){0,4}$)"
				if(findrx(_s pattern 0) < 0)
					TempArray = Configs[Config]
					_s = getTok(TempArray[0] 2 -1 ",")
					_s.setwintext(id(1113 hDlg))
				else
					TempArray.create(0)
					TempArray = Configs[Config]
					s1.getwintext(id(1113 hDlg))
					if (_s.len > 1)
						_s.format("%s," getTok(TempArray[0] 0 -1 ","))
						err	_s.format("10")
						_s.formata("%s," getTok(TempArray[0] 1 -1 ","))
						err	_s.formata("-1,")
						_s.formata("%s" s1)
						TempArray[0] = _s
						Configs[Config] = TempArray
	
	case EN_CHANGE<<16|1115 ;;Change V
		if (ControlModify)
			if (FExtension < 0)
				_s = ""
				_s.setwintext(id(1115 hDlg))
			else
				_s.getwintext(id(1115 hDlg))
				pattern = "(^\-1$)|(^-$)|(^([0-9]){0,2}$)"
				if(findrx(_s pattern 0) < 0)
					TempArray = Configs[Config]
					_s = getTok(TempArray[0] 1 -1 ",")
					_s.setwintext(id(1115 hDlg))
				else
					TempArray.create(0)
					TempArray = Configs[Config]
					s1.getwintext(id(1115 hDlg))
					if (_s.len > 1)
						_s.format("%s," getTok(TempArray[0] 0 -1 ","))
						err	_s.format(",")
						_s.formata("%s," s1)
						_s.formata("%s," getTok(TempArray[0] 2 -1 ","))
						err	_s.formata("-1,")
						_s.formata("%s" getTok(TempArray[0] 3 -1 ","))
						err	_s.formata("-1")
						TempArray[0] = _s
						Configs[Config] = TempArray
	
	case EN_CHANGE<<16|1116 ;;Change VD
		if (ControlModify)
			if (FExtension < 0)
				_s = ""
				_s.setwintext(id(1116 hDlg))
			else
				_s.getwintext(id(1116 hDlg))
				pattern = "(^\-1$)|(^-$)|(^([0-9]){0,4}$)"
				if(findrx(_s pattern 0) < 0)
					TempArray = Configs[Config]
					_s = getTok(TempArray[FExtension] 2 -1 ",")
					_s.setwintext(id(1116 hDlg))
				else
					TempArray.create(0)
					TempArray = Configs[Config]
					s1.getwintext(id(1116 hDlg))
					if (_s.len > 1)
						_s.format("%s," getTok(TempArray[FExtension] 0 -1 ","))
						err	_s.format(",")
						_s.formata("%s," getTok(TempArray[FExtension] 1 -1 ","))
						err	_s.formata(",10,")
						_s.formata("%s," s1)
						_s.formata("%s" getTok(TempArray[FExtension] 3 -1 ","))
						err	_s.formata("-1")
						TempArray[FExtension] = _s
						Configs[Config] = TempArray
	
	case EN_CHANGE<<16|1117 ;;Change FD
		if (ControlModify)
			if (Config < 0)
				_s = ""
				_s.setwintext(id(1117 hDlg))
			else
				_s.getwintext(id(1117 hDlg))
				pattern = "(^\-1$)|(^-$)|(^([0-9]){0,4}$)"
				if(findrx(_s pattern 0) < 0)
					TempArray = Configs[Config]
					_s = getTok(TempArray[0] 1 -1 ",")
					_s.setwintext(id(1117 hDlg))
				else
					TempArray.create(0)
					TempArray = Configs[Config]
					s1.getwintext(id(1117 hDlg))
					if (_s.len > 1)
						_s.format("%s," getTok(TempArray[0] 0 -1 ","))
						err	_s.format("10,")
						_s.format("%s," getTok(TempArray[0] 1 -1 ","))
						err	_s.formata("-1,")
						_s.formata("%s," getTok(TempArray[0] 2 -1 ","))
						err	_s.formata("-1")
						_s.formata("%s" s1)
						TempArray[0] = _s
						Configs[Config] = TempArray
	
	case EN_CHANGE<<16|1114 ;;Change File extension
		if (ControlModify)
			if (FExtension < 0)
				_s = ""
				_s.setwintext(id(1114 hDlg))
			else
				_s.getwintext(id(1114 hDlg))
				_s.trim
				pattern = "(^[0-9a-zA-Z-_~]{0,6}$)"
				if((findrx(_s pattern 0) < 0) & (_s.len>0))
					TempArray = Configs[Config]
					_s = getTok(TempArray[FExtension] 0 -1 ",")
					_s.setwintext(id(1114 hDlg))
				else
					TempArray.create(0)
					TempArray = Configs[Config]
					s1.getwintext(id(1114 hDlg))
					_s.format("%s," s1)
					if (_s.len > 1)
						_s.formata("%s," getTok(TempArray[FExtension] 1 -1 ","))
						err	_s.formata("10,")
						_s.formata("%s," getTok(TempArray[FExtension] 2 -1 ","))
						err	_s.formata("-1,")
						_s.formata("%s" getTok(TempArray[FExtension] 3 -1 ","))
						err	_s.formata("-1")
						TempArray[FExtension] = _s
						Configs[Config] = TempArray

	case EN_KILLFOCUS<<16|1114 ;; File extension. Focus lost
		if (ControlModify)
			if (FExtension < 0)
				_s = ""
				_s.setwintext(id(1114 hDlg))
			else
				_s.getwintext(id(1114 hDlg))
				_s.ucase(_s)
				_s.setwintext(id(1114 hDlg))

	case 1121 ;; Add Config
		ControlModify = 0
		Configs[]="10,-1,-1"
		Sources[]=" [] "
		Config = -1
		FExtension = -1
		SendMessage hlbC LB_RESETCONTENT 0 0
		SendMessage hlbF LB_RESETCONTENT 0 0
		_s = ""
		for i1 1109 1118
			_s.setwintext(id(i1 hDlg))
		for _i 0 Configs.len
			_s.format("Config%i" (_i+1))
			LB_Add(hlbC _s 0)
		ControlModify = 1

	case 1122 ;; Remove Config
		ControlModify = 0
		if (Config >= 0)
			Configs.remove(Config)
			Sources.remove(Config)
			Config = -1
			FExtension = -1
			SendMessage hlbC LB_RESETCONTENT 0 0
			SendMessage hlbF LB_RESETCONTENT 0 0
			for _i 1109 1118
				EnableWindow id(_i hDlg) 0
			_s = ""
			for i1 1109 1118
				_s.setwintext(id(i1 hDlg))
			for _i 0 Configs.len
				_s.format("Config%i" (_i+1))
				LB_Add(hlbC _s 0)
		ControlModify = 1

	case 1123 ;; Add Extension
		ControlModify = 0
		TempArray = Configs[Config]
		TempArray[]= " ,10,-1,-1,-1"
		Configs[Config] = TempArray
		FExtension = -1
		SendMessage hlbF LB_RESETCONTENT 0 0
		_s = ""
		for i1 1114 1118
			_s.setwintext(id(i1 hDlg))
		for i1 1 TempArray.len
			s1.format("%s" getTok(TempArray[i1] 0 -1 ","))
			s1.ucase(s1)
			_s.format("Filetype %s" s1)
			LB_Add(hlbF _s 0)
		ControlModify = 1
	
	case 1124 ;; Remove Extension
		ControlModify = 0
		if (Config >= 0)
			TempArray = Configs[Config]
			TempArray.remove(FExtension)
			Configs[Config] = TempArray
			FExtension = -1
			SendMessage hlbF LB_RESETCONTENT 0 0
			_s = ""
			for i1 1114 1118
				_s.setwintext(id(i1 hDlg))
				EnableWindow id(i1 hDlg) 0
			for i1 1 TempArray.len
				s1.format("%s" getTok(TempArray[i1] 0 -1 ","))
				s1.ucase(s1)
				_s.format("Filetype %s" s1)
				LB_Add(hlbF _s 0)
		ControlModify = 1
	
	case 3 ;; HELP
	_s = 
 A bit of help for this strange frontend....
 The purpose of this macro is create the INI configuration file, a bit more userfriendly than with notepad.
 
 I have separate into 3 steps.
 The first one is browse the file to create/modify and the file to log changes.
 Once selected a configuration file, Step 2 and 3 will be visible.
 About log file, there is little to say. Just records files copied and removed and maintenance (when versions are removed, they aren't recorded)
 
 In step 2 you must choose a \"configuration\" or rule. Each one can contain a source directory and a backup directory.
 If you want to backup some subdirs of a folder but not all, you must create a new rule for each one.
 Below the list you have two buttons: create and destroy.
 You must select some rule before modify anything in step 3.
 
 Step 3 is a bit more "complicated".
 First of all, you must setup the Versions, Delete Versions and Delete Files. (the left most ones)
 
 Versions: How many older copies/versions (maximum) from a file will have.
           It's a number between 0 and 99. Default: 10.
 Delete Versions: That means how much time (in days) older copies will remain on backup.
           Any version saved from a file older than X will be deleted.
           In this step, last copy will never deleted.
           It's a number between -1 and 9999, where -1 means never!
           For security reasons, any number between 0 and 9 will be 10. Default: -1
           The days are calculated since last modification.
 Delete Files: A complement for the last one.
           In this case, the number of days is only applied to those files which doesn't exist any more on the source.
           When the file is deleted, all his versions are deleted too.
           It's a number between -1 and 9999, where -1 means never!
           For security reasons, any number between 0 and 9 will be 10. Default: -1
           The days are calculated since last modification.
 
 These 3 datas are for all the files, but you can create exceptions (by file extension).
 That is the right part of this step.
 As in step 2, you have a list of extensions rules, which can be created or removed.
 Once you have selected an extension rule, you can setup his properties: Extension, Versions, delete versions and delete files.
 The extension can have a maximum of 6 characters (without dot).
 The other properties are the same as the global ones, except the versions (only explained changes):
 Versions: It's a number between -1 and 99. Default: -1.
           The meaning of -1 is NO backup this filetype! (useful for temporary files)
 
 I hope it can be helpful to you.
 

	mes _s

	case IDOK

	case IDCANCEL

ret 1

