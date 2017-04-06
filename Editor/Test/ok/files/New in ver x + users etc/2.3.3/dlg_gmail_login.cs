\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
ax3SHD=""
if(!ShowDialog("dlg_gmail_login" &dlg_gmail_login &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 533 313 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 532 312 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	goto load
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	Htm h=htm("body" "" "" hDlg)
	out h.HTML ;;<body>
	out h.DocText(1) ;;all. But with gmail it is very slow. Don't know why, slowly retrieves <head>, which in gmail is bigger than <body>.
	
	 ---------
	
	  another way
	 
	 SHDocVw.WebBrowser we3
	 we3._getcontrol(id(3 hDlg))
	 
	  <body>
	 MSHTML.IHTMLDocument2 doc2=we3.Document
	 out doc2.body.outerHTML
	 
	   all. Also slow with gmail.
	  MSHTML.IHTMLDocument3 doc3=we3.Document
	  out doc3.documentElement.outerHTML
	
	case IDCANCEL
ret 1

 load
 ____________________

 user data
str user="xxxxxxxxxxxxxxxxxxxx@gmail.com"
str password="xxxxxxxxxxxxxxxxxxx"

 ____________________

 load gmail. If not logged in, will load login page.
str url
opt waitmsg 1
web "http://mail.google.com/mail/" 1 hDlg "" url
 out url
if url.beg("https://www.google.com/accounts/ServiceLogin")
	 fill login form
	 out 1
	Htm el
	el=htm("INPUT" "Email" "" hDlg 0 12 0x121); el.SetText(user)
	el=htm("INPUT" "Passwd" "" hDlg 0 13 0x121); el.SetText(password)
	el=htm("INPUT" "signIn" "" hDlg 0 14 0x121); el.Click
	 out 2
