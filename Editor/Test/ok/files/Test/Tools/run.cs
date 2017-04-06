run "C:\Documents and Settings\G\Start Menu\System\Toolbars\T\WordPad.lnk" "C:\Documents and Settings\G\My Documents\Run.reg" "" "C:\Documents and Settings\All Users\Documents" 0xA00 win("app - Microsoft Visual C++" "IDEOwner")
run "control.exe" "firewall.cpl,Windows Firewall" "" "*"
run "???" "" "" "" 0x500
run "???" "" "" "" 0x200; 60 P; err
run "???"; err ret
60 P; err
int h
run "e:\MyProjects\app\qm.exe" "" "" "" 0x1000 win("QM TOOLBAR" "QM_toolbar") h

run "$Desktop$\led.txt" "" "" "$CD Burning$"
run "C:\Documents and Settings\All Users\Documents" "" "Explore"
run ":: 14001F50E04FD020EA3A6910A2D808002B30309D 14002E1E2020EC21EA3A6910A2DD08002B30309D 810000000000000020003E00433A5C57494E444F57535C73797374656D33325C6967667863706C2E63706C00496E74656C2852292045787472656D652047726170686963732032204D00436F6E74726F6C20746865206772617068696373206861726477617265206665617475726573206F6620796F75722073797374656D2E00"

cop+ "C:\Documents and Settings\G\Start Menu\System\Toolbars\T\Microsoft Word.lnk" "C:\Documents and Settings\G\My Documents" 0x2C0|FOF_NOERRORUI|FOF_SILENT
ren "C:\Documents and Settings\G\My Documents\Run.reg" "$Desktop$"
ren* "???" "???"
del "C:\Documents and Settings\G\My Documents\Run.reg"
