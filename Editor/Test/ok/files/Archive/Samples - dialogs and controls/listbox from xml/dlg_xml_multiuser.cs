\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "5 6"
str e5 e6
if(!ShowDialog("" &dlg_xml_multiuser &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 3 Static 0x54000000 0x0 4 6 48 12 "user"
 4 Static 0x54000000 0x0 4 22 48 12 "password"
 5 Edit 0x54030080 0x200 54 6 96 14 ""
 6 Edit 0x54030080 0x200 54 22 96 14 ""
 7 Button 0x54032000 0x0 4 42 48 14 "Add"
 8 Button 0x54032000 0x0 4 58 48 14 "Remove"
 9 ListBox 0x54230103 0x200 54 42 96 90 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
lpstr xfolder="$personal$\x493" ;;change this
lpstr xfile="$personal$\x493\x493.xml" ;;and this

sel message
	case WM_INITDIALOG
	if(!dir(xfile)) ;;if the file does not exist, create
		mkdir xfolder
		_s="<x />"
		_s.setfile(xfile)
	else ;;populate listbox
		update_listbox_from_xml hDlg xfile
		
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 7 ;;Add
	 get text from edit boxes
	str u p
	u.getwintext(id(5 hDlg))
	p.getwintext(id(6 hDlg))
	if(empty(u) or empty(p)) ret
	 save to file
	IXml x=CreateXml; x.FromFile(xfile)
	IXmlNode n=x.RootElement.Add("i")
	n.Add("u" u)
	n.Add("p" p)
	x.ToFile(xfile)
	 update listbox
	update_listbox_from_xml hDlg xfile
	
	case 8 ;;Remove
	int i=LB_SelectedItem(id(9 hDlg)); if(i<0) ret
	i=SendMessage(id(9 hDlg) LB_GETITEMDATA i 0)
	 remove from file
	x=CreateXml; x.FromFile(xfile)
	n=x.RootElement.Child("i" i+i)
	x.Delete(n)
	x.ToFile(xfile)
	update_listbox_from_xml hDlg xfile
	
	case IDOK
	case IDCANCEL
ret 1
