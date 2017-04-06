    These possibly could be on normal PC too, don't know.

Win8-64bit RC on VmWare: File trigger stopped working.
   Trigger info: Folder: C: + subfolders. Events: all.
   Stopped 2 times. During installation of some big software. After restarting QM worked, but stopped again after several minutes. Later worked all time.

____________________________________

	These were tested or almost sure that is due to Virtual PC

On XP, cannot unlock computer if QM runs from host PC. Even does not write to the log file. On other OS works.

On Vista, raw input does not recognize multiple keyboards.

mes etc: clicking does not stop countdown.

win7 64bit rc1 on vmware: expandfolder menus does not show control panel items. Win8 too (tested only 64-bit).
   IShellFolder::EnumObjects/Next does not get Control Panel items. Tested all new flags.
   Don't know, maybe on real PC too. Works well on 32bit real pc.

VPC, XP: Ftp.Dir does not get results.

vmware Vista: QM crashes on mouse left-right movement trigger. Only on Vista, only when QM runs from app.

wmware Windows 7 64-bit: QM crashes on paste in code editor. Also on text change in a Scintilla window, eg Resources.
	Not always. Ok after restarting Windows.
