\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD="www.google.com"
if(!ShowDialog("Dialog81" &Dialog81 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 465 361 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 468 346 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 347 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030103 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	str js=
 javascript:(function(){var d=document,s=d.createElement('script');s.src='http://mrclay.org/js/bookmarklets/myPage_1_1.min.js';s.type='text/javascript';d.body.appendChild(s)})();
	web js 0 hDlg
	case IDOK
	case IDCANCEL
ret 1
