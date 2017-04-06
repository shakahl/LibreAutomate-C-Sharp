 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 275 190 "Select Files and Folders"
 4 Static 0x54000000 0x0 6 6 264 36 "Enter one or more paths of files or folders.[][]You can select one or more files or folders in Explorer, and press F11.[]Repeat with other files/folders, if need."
 3 Edit 0x54231044 0x200 6 44 264 122 ""
 1 Button 0x54030001 0x4 90 172 48 14 "OK"
 2 Button 0x54030000 0x4 142 172 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	if(!GetParent(hDlg)) ont hDlg
	__RegisterHotKey-- hk.Register(hDlg 1 0 VK_F11)
	
	case WM_DESTROY
	hk.Unregister
	
	case WM_HOTKEY
	int hwnd=win()
	 Get ShellBrowserWindow interface by enumerating shell windows
	SHDocVw.ShellWindows sw._create
	SHDocVw.ShellBrowserWindow b
	foreach(b sw)
		int hwnd2=b.HWND; err continue
		if(hwnd=hwnd2) goto g1
	ret
	 g1
	str s.getwintext(id(3 hDlg))
	 get shell folder view document
	Shell32.ShellFolderView sfw=b.Document; err ret
	Shell32.FolderItem fi
	 enumerate selected items
	foreach(fi sfw.SelectedItems) s.addline(fi.Path)
	 add to textbox
	EditReplaceSel hDlg 3 s 1
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
