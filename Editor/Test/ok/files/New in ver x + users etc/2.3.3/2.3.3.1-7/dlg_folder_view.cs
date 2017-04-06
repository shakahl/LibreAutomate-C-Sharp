\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 441 374 "Form"
 3 ActiveX 0x54000000 0x0 0 20 440 352 "SHDocVw.WebBrowser"
 4 ComboBox 0x54230243 0x0 2 4 96 213 ""
 5 Button 0x54032000 0x0 116 2 64 14 "Open selected"
 6 Edit 0x44000080 0x200 300 0 34 14 ""
 7 Button 0x54032000 0x0 182 2 48 14 "Up"
 8 Button 0x54032000 0x0 233 2 48 14 "Sort"
 1 Button 0x54030001 0x0 338 2 48 14 "OK"
 2 Button 0x54030000 0x0 388 2 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "" "" "" ""

str controls = "3 4 6"
str ax3SHD cb4 e6
cb4="FVM_ICON[]FVM_SMALLICON[]FVM_LIST[]FVM_DETAILS[]FVM_THUMBNAIL[]FVM_TILE[]FVM_THUMBSTRIP"; if(_winver>=0x601) cb4+"[]FVM_CONTENT"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret
out e6


#sub DlgProc
function# hDlg message wParam lParam
SHDocVw.WebBrowser we3
Shell32.ShellFolderView fv
Shell32.FolderItem fi
sel message
	case WM_INITDIALOG
	we3._getcontrol(id(3 hDlg))
	we3._setevents("sub.we3") ;;need DocumentComplete event
	
	 initial folder
	sub.OpenFolder(we3 "$personal$")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
we3._getcontrol(id(3 hDlg))
sel wParam
	case CBN_SELENDOK<<16|4
	_i=CB_SelectedItem(lParam)+1
	fv=we3.Document
	fv.CurrentViewMode=_i
	
	case 5 ;;open selected
	fv=we3.Document
	fi=fv.FocusedItem
	 out fi.Path
	if fi.IsFolder
		sub.OpenFolder(we3 fi.Path)
	else
		fi.InvokeVerb
	
	case 7 ;;Up
	 we3.GoBack ;;error
	 we3.ExecWB ?
	_s=we3.LocationURL
	_s.getpath
	sub.OpenFolder(we3 _s)
	
	case 8 ;;Sort
	fv=we3.Document
	out fv.SortColumns ;;to get column names like below, manually sort a column and then run this. Names for descending sorting are with "-", and this code shows it.
	fv.SortColumns="prop:System.ItemTypeText;"
	
	case IDOK
	 get path of selected item
	fv=we3.Document
	fi=fv.FocusedItem
	str path=fi.Path
	 out path
	path.setwintext(id(6 hDlg))
	
ret 1


#sub OpenFolder
function SHDocVw.WebBrowser&we3 $folder
str s.expandpath(folder)
we3.Navigate(s)


#sub we3_DocumentComplete
function IDispatch'pDisp `&URL SHDocVw.IWebBrowser2'we3
 out __FUNCTION__
Shell32.ShellFolderView fv=we3.Document
fv._setevents("sub.fv")


#sub fv_SelectionChanged
function Shell32.IShellFolderViewDual3'fv
Shell32.FolderItem fi=fv.FocusedItem
str path=fi.Path
out path
