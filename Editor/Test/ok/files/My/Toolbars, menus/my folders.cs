 /expandfolders 0x20 0
My Computer :run "$17$"
C: :run "C:\"
Q: :run "Q:\"
Documents :run "$documents$"
Doc in Q :run "Q:\Doc"
User data (G) :run "$40$"
Program Files :run "$Program Files$"
Control Panel :run "$3$"
Recycle Bin :run "$10$"
 CD :RunCD * shell32.dll * 11
 Network :run "$18$"
-
Backup :run "%backup%" * folder.ico
VM :run "%VM%" * folder.ico
-
Windows :run "$windows$"
System :run "$system$"
System\Tasks :run "$system$\Tasks"
.NET :run "$windows$\Microsoft.NET\Framework"
 jdk1.8.0_11 :run "$program files$\Java\jdk1.8.0_11"
-
Temp :run "$temp$"
Temp QM :run "$temp qm$"
Knygos :run "%doc%\Knygos"
foto :run "%doc%\foto"
Lumix :run "J:\DCIM\100_PANA"
Mantra :run "$my music$\Mantra"
-
QM user appdata :run "$appdata$\GinDi\Quick Macros"
QM common appdata :run "$common appdata$\GinDi\Quick Macros"
QM HTMLHelp :run "$qm$\HTMLHelp"
QM in PF :run "$Program Files$\Quick Macros 2"
 test (in desktop) :run "$desktop$\test"
-
Apps "shell:AppsFolder" * folder.ico
