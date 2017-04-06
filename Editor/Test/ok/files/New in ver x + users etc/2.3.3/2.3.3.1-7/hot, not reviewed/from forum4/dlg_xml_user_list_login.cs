\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "5 6"
str e5 e6
if(!ShowDialog("" &dlg_xml_user_list_login &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 204 190 "Dialogin"
 3 Static 0x54000000 0x0 4 6 48 12 "user"
 4 Static 0x54000000 0x0 4 22 48 12 "password"
 5 Edit 0x54030080 0x200 54 6 96 14 ""
 6 Edit 0x54030080 0x200 54 22 96 14 ""
 7 Button 0x54032000 0x0 4 44 48 14 "Add"
 8 Button 0x54032000 0x0 4 62 48 14 "Remove"
 9 ListBox 0x54230103 0x200 54 44 96 141 ""
 10 Button 0x54032000 0x0 152 170 48 14 "Login"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "" "" ""


ret
 messages
lpstr xfolder="$personal$\Dialogin" ;;change this
lpstr xfile="$personal$\Dialogin\YTlogin2.xml" ;;and this

sel message
	case WM_INITDIALOG
	 when opening the dialog, make sure that the folder and the file exist
	if(!dir(xfile))
		mkdir xfolder
		_s="<x />"
		_s.setfile(xfile)
	else
		 if already exists, populate listbox
		update_listbox_from_xml2 hDlg xfile
		
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
str u p
IXml x; IXmlNode n
sel wParam
	case LBN_DBLCLK<<16|9
	 g1
	_i=LB_SelectedItem(lParam)
	 out _i
	if(_i<0) ret
	
	LB_GetItemText(lParam _i u)
	x=CreateXml; x.FromFile(xfile)
	n=x.Path(F"x/i[@u='{u}']") ;;get node 'i' whose attribute 'u' matches the selected user name
	p=n.AttributeValue("p")
	out F"u={u}, p={p}"; ret ;;disable this line
	
	opt waitmsg 1
	run "firefox.exe"
	wait 1
	AutoPassword u p 2 0 10
	key TVY
	
	case 7 ;;Add
	 get text from edit boxes
	u.getwintext(id(5 hDlg))
	p.getwintext(id(6 hDlg))
	if(empty(u) or empty(p)) ret
	 save to file
	x=CreateXml; x.FromFile(xfile)
	n=x.RootElement.Add("i")
	n.SetAttribute("u" u)
	n.SetAttribute("p" p)
	x.ToFile(xfile)
	 update listbox
	update_listbox_from_xml2 hDlg xfile
	
	case 8 ;;Remove
	int i=LB_SelectedItem(id(9 hDlg)); if(i<0) ret
	i=SendMessage(id(9 hDlg) LB_GETITEMDATA i 0)
	 remove from file
	x=CreateXml; x.FromFile(xfile)
	n=x.RootElement.Child("i" i+i)
	x.Delete(n)
	x.ToFile(xfile)
	update_listbox_from_xml2 hDlg xfile
	
	case IDOK
	case IDCANCEL
	case 10
	lParam=id(9 hDlg); goto g1
ret 1