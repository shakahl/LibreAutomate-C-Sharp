\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=""
if(!ShowDialog("" &Dialog90 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 529 306 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 530 304 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	goto g1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 g1

out
 str url="http://rapidshare.com/files/342182548/Cambridge_Advanced_Learners_Dictionary.part4.rar"
 str url="http://www.google.lt"
 IntGetFile url shtml

str shtml
 shtml="<html><head></head><body><p>test ąčę test</p></body></html>"
 shtml=F"<html><head></head><body><p>test ąčę test</p><pre>{_s.getmacro()}</pre><script>alert(''script'');</script></body></html>"
shtml="<html><head><script src=''test.js''></script></head><body><p>test ąčę test</p><img src=''record.png''></body></html>"
 _s.expandpath("$temp$\test.js"); shtml=F"<html><head><script src={_s}></script></head><body><p>test ąčę test</p><img src=''record.png''></body></html>"

_s="alert(''script'');"; _s.setfile("$temp$\test.js")

 HtmlToWebBrowserControl id(3 hDlg) shtml
HtmlToWebBrowserControl id(3 hDlg) shtml 0 0 "$temp$"
 HtmlToWebBrowserControl id(3 hDlg) shtml 0 0 "$temp$"
