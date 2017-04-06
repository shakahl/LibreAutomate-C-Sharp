 /exe
 \Dialog_Editor

 info: this exe uses test.manifest, where dpiAware is False.

dll- user32 #PhysicalToLogicalPointForPerMonitorDPI hWnd POINT*lpPoint
dll- user32 #LogicalToPhysicalPointForPerMonitorDPI hWnd POINT*lpPoint

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Acc in-process"
 3 Button 0x54032000 0x0 0 0 48 14 "Button"
 4 ListBox 0x54230101 0x200 32 32 96 48 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "4"
str lb4
lb4="one[]two[]three[]four[]five"
int w=win("" "QM_Editor")
if(!ShowDialog(dd &sub.DlgProc &controls w 0 0 0 0 -1 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 1 0
	SetWindowSubclass(hDlg &sub.WndProc_Subclass 1 0)
	 SetWindowSubclass(id(4 hDlg) &sub.WndProc_Subclass 2 0)
	
	case WM_TIMER
	sel wParam
		case 1; KillTimer hDlg 1; act win("" "QM_Editor")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	 case WM_APP
	 POINT p.x=wParam; p.y=lParam
	 if(!PhysicalToLogicalPointForPerMonitorDPI(hDlg &p)) ret
	  out "%i %i" p.x p.y
	 Acc a.FromXY(p.x p.y)
	  out a.Name
	 int cx cy; a.Location(p.x p.y cx cy)
	 if(!LogicalToPhysicalPointForPerMonitorDPI(hDlg &p)) ret
	
	 case WM_GETOBJECT
	 out "0x%X 0x%X" wParam lParam
	
	 TODO: QM Debug build asserts (corrupt memory) if error "unknown identifier" here when starting (autobuilding) exe.
	
	 ret DT_Ret(hDlg MakeInt(p.x p.y))
ret
 messages2
sel wParam
	case 3
	out DpiIsWindowScaled(1)
ret 1

 BEGIN PROJECT
 main_function  dialog_DPI_unaware_exe
 exe_file  $my qm$\dialog_DPI_unaware_exe.exe
 icon  <default>
 manifest  $qm$\test.manifest
 on_before  MakeExeCloseRunning
 flags  6
 guid  {52D4EA6C-9A7C-406E-B058-AA8B9C705698}
 END PROJECT
 manifest  $qm$\default.exe.manifest


#sub WndProc_Subclass
function# hWnd message wParam lParam uIdSubclass dwRefData

sel message
	case WM_APP+1
	Acc a.Find(hWnd "LISTITEM" "three")
	 out a.a
	int i=LresultFromObject(IID_IAccessible wParam a.a)
	 if(i<0) out "LresultFromObject failed"
	ret i
	 how to return elem?

int R=DefSubclassProc(hWnd message wParam lParam)

sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hWnd &sub.WndProc_Subclass uIdSubclass)
	
	 case WM_GETOBJECT
	 outw hWnd "" _s
	 out "0x%X 0x%X  ret=0x%X  %s" wParam lParam R _s

ret R
