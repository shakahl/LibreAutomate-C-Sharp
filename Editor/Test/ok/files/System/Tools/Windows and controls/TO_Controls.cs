 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
message=val(_command); if(message) goto gLinks

str controls = "7 5"
__strt qmt7 lb5

lb5="&About[]Button, check box[]ComboBox[]ListBox[]Other controls"

if(!ShowDialog("" &TO_Controls &controls _hwndqm)) ret

str s winVar
__strt vd

qmt7.Win(winVar)
s=F"{vd.VD(`int c`)}={winVar}"
qmt7.WinEnd(s)

InsertStatement s
ret

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 248 162 "Controls"
 3 Static 0x50000000 0x0 6 36 130 16 ""
 7 QM_Tools 0x54030000 0x10000 4 4 240 54 "1 16 2"
 5 ListBox 0x54230101 0x200 4 68 78 62 ""
 4 QM_DlgInfo 0x54000000 0x20000 92 68 152 62 ""
 1 Button 0x54030001 0x4 2 144 48 14 "OK"
 2 Button 0x54010000 0x4 52 144 50 14 "Cancel"
 6 Static 0x54000010 0x20004 0 138 286 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

 messages
if(sub_to.ToolDlgCommon(&hDlg "3[]$qm$\controls.ico" "" 1)) ret wParam
sel message
	case WM_INITDIALOG goto gAction
	case WM_COMMAND goto messages2
	case __TWN_DRAGEND goto gFinderDrop
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|5 goto gAction
ret 1

 gAction
sel TO_Selected(hDlg 5)
	case 0
	s=
 <macro "TO_Controls /1">About this dialog</macro>
 <macro "TO_Controls /2">About controls</macro>
 <macro "TO_Controls /3">About other UI objects</macro>
 <macro "TO_Controls /4">About control functions and messages</macro>
	case 1
	s=
 Use function <help>but</help> with buttons and checkboxes. Can click, check, get checked state.
;
 <macro "TO_Controls /5">Examples</macro>
	case 2
	s=
 <help>CB_SelectedItem</help>
 <help>CB_SelectItem</help>
 <help>CB_SelectString</help>
 <help>CB_FindItem</help>
 <help>CB_GetCount</help>
 <help>CB_GetItemText</help>
 <help>CB_Add</help>
	case 3
	s=
 <help>LB_SelectedItem</help>
 <help>LB_SelectItem</help>
 <help>LB_SelectString</help>
 <help>LB_FindItem</help>
 <help>LB_GetCount</help>
 <help>LB_GetItemText</help>
 <help>LB_Add</help>
	case 4
	s=
 <help>SelectTab</help>
 <help>GetListViewItemText</help>
 <help>GetStatusBarText</help>

SetDlgItemText hDlg 4 F"<>{s}"
ret

 gLinks
sel message
	case 1
	s=
 The 'Controls' dialog does not create code for control functions. It just gives information. Also creates code to get captured control handle that can be used with control functions and messages.
	case 2
	s=
 Controls are those UI objects in windows that you can capture with the Drag tool. There are many types of controls - buttons, edit controls, etc. Controls are <help #IDP_WINDOWSTYLES>child windows</help>. You can use most window functions with them; use the 'Window' dialog.
	case 3
	s=
 Controls and other UI objects usually are <help #IDP_ACCESSIBLE>accessible objects</help>. You can use accessible object dialogs. Advantages: more objects, more functions. Disadvantages: slower.
;
 UI objects in web pages are not child windows. Use accessible object dialogs. With Internet Explorer - also HTML element dialogs.
;
 With MS Excel you can use <help>ExcelSheet</help> class. With other MS Office programs - COM interfaces that they provide.
;
 Some UI objects cannot be captured as controls or other objects. Then try dialogs 'Find Image' and 'Window Text'.
	case 4
	s=
 QM has several functions that can be used with some standard Windows controls. You can find them in 'Controls' dialog and in 'control' <help #IDP_CATEGORIES>category</help>.
;
 Most QM control functions can be used with controls of other applications as well as with controls of your dialogs.
;
 With standard Windows controls you also can use control messages. Documented in the <help #IDP_MSDN>MSDN Library</help>. To use strings or pointers with controls of other applications, need <help>__ProcessMemory</help> class.
	case 5
	s=
 <code>
  get control handle. To create this code, use 'Controls' or 'Find window or control' dialog.
 int w=win("Some Window" "#32770") ;;window
 int c=id(20 w) ;;control
  then use control functions. Examples:
 act w ;;activate window, or the 'click' functions may not work. The 'check' functions work always.
 but c ;;click. Can be used with buttons and checkboxes.
 but+ c ;;check. Also can be used to click button. Don't need active window. Does not set focus.
 if but(c) ;;is checked?
 	out "checked"
 	but- c ;;uncheck
 </code>
QmHelp s 0 6
ret

 gFinderDrop
__MapIntStr m.AddList("0 ListBox[]2 Button[]3 Static[]4 Edit[]5 ComboBox[]10 ScrollBar[]11 StatusBar[]12 ToolBar[]13 Progress[]14 Animate[]15 Tab[]16 HotKey[]17 Header[]18 TrackBar[]19 ListView[]22 UpDown[]24 ToolTips[]25 TreeView[]28 RichEdit")
int ct=SendMessage(wParam WM_GETOBJECT 0 OBJID_QUERYCLASSNAMEIDX)-65536
ct=m.FindInt(ct)
if ct>=0
	s=m.a[ct].s
	 g1
	if(ct=1) sel(GetWinStyle(wParam)&BS_TYPEMASK) case [2,3,4,5,6,9] s+". Check box."; case 7 s+". Group box."; case BS_OWNERDRAW s+". Owner-draw."
else
	sel s.getwinclass(wParam) 3
		case "ComboBoxEx32" s="ComboBoxEx"
		case "ReBarWindow32" s="ReBar"
		case "SysDateTimePick32" s="DateTime"
		case "SysIPAddress32" s="IPAddress"
		case "SysMonthCal32" s="MonthCalendar"
		case "SysPager" s="Pager"
		case "SysLink"
		case "RichEdit*" s="RichEdit"
		case "#32770" s="Dialog"; if(IsChildWindow(wParam) and IsWindow(SendMessage(GetParent(wParam) PSM_INDEXTOHWND 0 0))) s+". A property sheet page."
		case else
		_s.GetWinBaseClass(wParam)
		if(_s!s) s=_s; if(s="Button") ct=1; goto g1
		else s=""
if(s.len) s-"Control type: "
SetDlgItemText hDlg 3 s
ret

 note:
 .NET USER32-based controls don't respond to either wm_getobject or realgetwindowclass.
 Don't know, maybe it depends on version, or on whether themed or not.
