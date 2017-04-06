\Dialog_Editor

 note: Many sample pages don't respond to clicks etc as in browse.exe, because custom behaviors not implemented. Would need to convert much C++ code from HTMLayout SDK.
 note: hyperlinks don't work here, although work in windowless.
 HTMLayout probably would be too difficult for QM users. Currently not in forum.
 Also wanted to test the ActiveX, but did not find the dll, and failed to compile the project.

out
str- samplesFolder="Q:\Downloads\HTMLayoutSDK\html_samples"

ref HTMLayout "__HTMLayout_API" 1

str controls = "4"
str lb4

ARRAY(str) a; GetFilesInFolder a samplesFolder "*.htm" 4
for(_i 0 a.len) lb4.addline(a[_i]+samplesFolder.len+1)

if(!ShowDialog("" &sub.DlgProc &controls)) ret

 BEGIN DIALOG
 1 "" 0x90CF0AC8 0x0 0 0 654 334 "Dialog"
 4 ListBox 0x54230101 0x200 0 0 154 334 ""
 3 Static 0x54000200 0x0 156 0 498 334 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040108 "*" "" "" ""

#sub DlgProc r
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	int h=id(3 hDlg)
	RECT rc; GetClientRect h &rc; MapWindowPoints h hDlg +&rc 2
	h=CreateWindowEx(0 HTMLayoutClassName 0 WS_CHILD|WS_VISIBLE rc.left rc.top rc.right-rc.left rc.bottom-rc.top hDlg 999 _hinst 0)
	HTMLayoutSetCallback(h &sub.Notify +hDlg)
	HTMLayoutWindowAttachEventHandler(h &sub.Event 0 -1)
	DT_SetAutoSizeControls hDlg "4sv 999s"
	
	_s=
	 <html><head><meta http-equiv="Content-Type" content="text/html; charset=utf-8"></head>
	 <body>test <b>bold</b> <a href=''http://www.quickmacros.com''>quickmacros.com</a> ąčę א
	 </body></html>
	HTMLayoutLoadHtml(h _s _s.len)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
str- samplesFolder
sel wParam
	case LBN_SELCHANGE<<16|4
	_i=LB_SelectedItem(lParam)
	LB_GetItemText(lParam _i _s)
	_s-"\"; _s-samplesFolder
	h=id(999 hDlg)
	HTMLayoutLoadFile(h @_s)
	
	case IDOK
	case IDCANCEL
ret 1


#sub Notify r
function# uMsg wParam lParam hDlg
NMHDR* nh=+lParam
 out nh.code
sel nh.code
	 case HLN_DOCUMENT_COMPLETE
	case HLN_ATTACH_BEHAVIOR
	NMHL_ATTACH_BEHAVIOR* nab=+nh
	out nab.behaviorName
	 ret DT_Ret(hDlg 1)


#sub Event r
function! !*tag he evtg BEHAVIOR_EVENT_PARAMS*p
 out p.cmd
sel p.cmd
	case BUTTON_CLICK out "BUTTON_CLICK: %s" sub.id_or_name_or_text(p.heTarget)
	case HYPERLINK_CLICK out "HYPERLINK_CLICK: %s" sub.get_attribute(p.heTarget "href") ;;does not work


#sub get_attribute r
function~ he $attrName
word* w
HTMLayoutGetAttributeByName(he attrName &w)
ret _s.ansi(w)


#sub id_or_name_or_text r
function~ he

word* w; lpstr s
HTMLayoutGetAttributeByName(he "id" &w)
if(!w) HTMLayoutGetAttributeByName(he "name" &w)
if(w) ret _s.ansi(w)
HTMLayoutGetElementInnerText(he &s)
ret _s.from("{" s "}")
