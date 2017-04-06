 /exe
__RegisterHotKey hk1.Register2(0 1 "CF10") ;;Ctrl+F10
__RegisterHotKey hk2.Register2(0 2 "CF11")
MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	if(m.message!=WM_HOTKEY) continue
	
	 edit this code if need. The numbers must match the numbers passed to Register2.
	sel m.wParam
		case 1
		sub.install ;;or can be: mac "sub.install"
		
		case 2
		sub.uninstall ;;or can be: mac "sub.uninstall"


#sub install
out 1


#sub uninstall
out 2


 BEGIN PROJECT
 main_function  Macro2766
 exe_file  $my qm$\Macro2766.qmm
 flags  6
 guid  {A0ECEC7F-3E04-4314-A0EE-9A4838CAC8D0}
 END PROJECT
