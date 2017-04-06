/exe 1
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("DlgDropTarget" &DlgDropTarget 0 _hwndqm 0 0 0 0 1 -1)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 0 84 76 61 "OK"
 2 Button 0x54030000 0x4 108 88 70 50 "Cancel"
 3 Button 0x54032000 0x0 6 6 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020006 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 QmRegisterDropTarget(hDlg hDlg 16|32)
	 QmRegisterDropTarget(id(1 hDlg) hDlg 16)
	 QmRegisterDropTarget(id(2 hDlg) hDlg 16)
	 out QmRegisterDropTarget(id(2201 _hwndqm) hDlg 16)
	 out QmRegisterDropTarget(win("Notepad") hDlg 16|0x8000)
	 out QmRegisterDropTarget(win("Notepad") hDlg 16)
	QmRegisterDropTarget(hDlg hDlg 1)
	EnableWindow id(1 hDlg) 0
	 EnableWindow hDlg 0
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_QM_DRAGDROP
	QMDRAGDROPINFO& di=+lParam
	out di.files
	 out "%i %i" di.dataObj di.formats
	 out di.hwndTarget
	 out GetDlgCtrlID(di.hwndTarget)
	 out di.effect
	 di.effect=DROPEFFECT_COPY
	 out di.pt.y
	 out di.keyState
	
#opt nowarnings 1
	out wParam
	if(wParam=0)
		for(_i 0 di.formats.len) out di.formats[_i].cfFormat
		STGMEDIUM sm
		di.dataObj.GetData(&di.formats[0] &sm); err ret
		out sm.hGlobal
		 sm.hGlobal=0
		ReleaseStgMedium(&sm)
	
	ret DT_Ret(hDlg 1)
ret
 messages2
sel wParam
	case 3
	 mes 1
	MessageBox hDlg "" "" 0
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  DlgDropTarget
 exe_file  $my qm$\DlgDropTarget.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {56726592-D9E2-43BF-B8AB-5DA0F181B2E5}
 END PROJECT
