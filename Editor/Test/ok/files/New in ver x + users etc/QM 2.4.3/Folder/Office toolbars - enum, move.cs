 /exe 1
out

typelib Word {00020905-0000-0000-C000-000000000046} 8.0
Word.Application a._getactive ;;connect to Word. On Vista/7, macro process must run as User. QM normally runs as Admin. The /exe 1 tells QM to run the macro in separate process, as User.

typelib Office {2DF8D04C-5BFA-101B-BDE5-00AA0044DE52} 2.3
Office.CommandBars bc=a.CommandBars
Office.CommandBar b

foreach b bc
	if(!b.Visible) continue
	str name=b.Name
	int x(b.Left) y(b.Top)
	out F"{name}  {x} {y}"

b=bc.Item("Web")
b.Left=b.Left+25


 BEGIN PROJECT
 main_function  Macro2463
 exe_file  $my qm$\Macro2463.qmm
 flags  6
 guid  {11405A8C-AA8D-46E3-A711-E2E2F680C2DA}
 END PROJECT
