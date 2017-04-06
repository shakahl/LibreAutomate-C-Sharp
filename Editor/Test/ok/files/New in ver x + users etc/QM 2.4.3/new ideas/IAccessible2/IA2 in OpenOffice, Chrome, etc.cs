/exe
 out
typelib IA2 "$qm$\IA2.tlb"
def IID_IAccessible2 uuidof(IA2.IAccessible2)
 atend IA2uninstall; if(!IA2install) ret
 IA2install does not work because also need to exec this in that process.
 If the IA2 dll is registered, don't need this. The dll can be anywhere, eg in system32 or in app dir.

 int w=win("Google Chrome" "Chrome_WidgetWin_1")
 int w=win("Firefox" "Mozilla*WindowClass" "" 0x4)
 int w=win("Opera" "OperaWindowClass") ;;no
 int w=win("Eclipse" "SWT_Window0") ;;no
int w=win("LibreOffice" "SALFRAME")
 int w=win("OpenOffice Writer" "SALFRAME")
 2; int w=win(mouse)

PF
Acc a a2
a.FromWindow(w) ;;just to auto-enable accessibility in OO and LO
a.FromWindow(w OBJID_CLIENT) ;;need OBJID_CLIENT to get IA2
PN
IServiceProvider pService
a.a.QueryInterface(IID_IServiceProvider &pService)
pService.QueryService(IID_IAccessible, uuidof(IA2.IAccessible2), &a2.a);
err out _error.description
 pService.QueryService(IID_IAccessible, uuidof(IA2.IAccessibleApplication), &a2.a);
PN;PO
out "%i %i" a.a a2.a
 a2.showTree(0)

 or use __QueryService

 BEGIN PROJECT
 main_function  IA2 in OpenOffice
 exe_file  $my qm$\IA2 in OpenOffice.qmm
 flags  6
 guid  {1883ACF3-92AB-4DF4-9285-63298E6A0BE4}
 END PROJECT
