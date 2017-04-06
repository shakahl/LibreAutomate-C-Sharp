 /exe
out
act "Notepad"
 act "Word"
 act "Dreamweaver"
 act "Firefox"
 act "Internet Explorer"
 act "Write"

key CaX
0.2
spe
Q &q
opt keysync 1
 rep 3
	 key "qwertyuiop asdfghjkl zxcvbnm[]"
 opt keysync 0
opt keysync 0
Q &qq
rep 10
	key "qwertyuiop asdfghjkl zxcvbnm[]"
Q &qqq
outq
0.2
act _hwndqm

 BEGIN PROJECT
 main_function  Macro1705
 exe_file  $my qm$\Macro1705.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {25839060-D6E8-4A81-AD01-73FCF6551228}
 END PROJECT
