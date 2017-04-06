\Dialog_Editor

str controls = "3"
str lb3

str exe s

if(len(_command))
	exe=_command
	exe.trim("''")
	exe.dospath
	goto g1

 Actually only qm.exe, x64\qmphook.sys and quickmac.exe must be signed. Don't sign qmvistad.exe.
 s="qm.exe[]qmcl.exe[]qmtul.exe[]x64\qm64.exe[]x64\qmphook.sys[][]quickmac.exe"
s="qm.exe[]qmcl.exe[]x64\qmphook.sys[][]quickmac.exe"
lb3=s

if(!ShowDialog("SignCodeDialog" 0 &controls)) ret

int i hqm
for i 0 lb3.len
	if(lb3[i]='1')
		exe.getl(s i)
		if(!exe.len) continue
		 g1
		sel exe 3
			case ["qm.exe","*\qm.exe"]
#if EXE
			hqm=win("" "QM_Editor")
			if(hqm)
				men 101 win("" "QM_Editor") ;;Exit Program
				wait 30 WP hqm
				2
#else
			run "$qm$\SignCodeDialog.exe" _s.getfilename(exe 1)
			ret
#endif
		SignCode exe
		if(hqm) run "$qm$\qm.exe" "v"


 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 108 164 "Sign Code"
 3 ListBox 0x54230109 0x200 4 20 102 88 ""
 1 Button 0x54030001 0x4 4 148 48 14 "OK"
 2 Button 0x54030000 0x4 56 148 48 14 "Cancel"
 4 Static 0x54000000 0x0 4 4 100 13 "Select files to sign:"
 5 Static 0x54000000 0x0 4 114 102 28 "Note: To sign qm.exe from QM, QM will be automatically restarted."
 END DIALOG
 DIALOG EDITOR: "" 0x2020001 "" ""

 BEGIN PROJECT
 main_function  SignCodeDialog
 exe_file  $qm$\SignCodeDialog.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {79BFDA73-0C06-48F4-9F68-ED4AC206F7D6}
 END PROJECT
