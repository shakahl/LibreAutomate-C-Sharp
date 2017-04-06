hh :run "hh.exe"
 hh :run "notepad.exe"
hh :run "notepad.exe" * hh.exe,0
 hh :run "notepad.exe" * .chm
QM Help :run "%app%\QM2Help.chm" "" "" "%app%"
 Text :mac "TO_Text" * $qm$\text.ico
exe no icon :run "$qm$\free_qmhook.exe"
nopath :run "notepad.exe" * imageres.dll,2
