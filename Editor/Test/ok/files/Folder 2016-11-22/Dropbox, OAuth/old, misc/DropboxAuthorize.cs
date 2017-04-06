 /
function'str $appKey $appSecret

 Authorizes your Dropbox application (actually the caller macro) to use Dropbox API with user's Dropbox account.
 Returns an access token string that then can be used with Dropbox API.

 appKey - from https://www.dropbox.com/developers/apps -> your app -> App key.
 appSecret - from https://www.dropbox.com/developers/apps -> your app -> App secret.

 REMARKS
 Opens https://www.dropbox.com/1/oauth2/authorize?client_id={appKey}... in a hidden web browser control, clicks Allow and gets token from next page.
 First time in QM process shows Dropbox sign-in page, let the user enter Dropbox email and password.
 Error if fails, for example if the user closes the email/password dialog.

 EXAMPLE
 out
 str token=DropboxAuthorize("myAppKey" "myAppSecret")
 
 typelib WinHttp {662901FC-6951-4854-9EB2-D9A2570F2B2E} 5.1
 WinHttp.WinHttpRequest r._create
 r.Open("POST" "https://api.dropboxapi.com/2/users/get_current_account")
 r.SetRequestHeader("Authorization" F"Bearer {token}")
 r.Send()
 ARRAY(byte) a=r.ResponseBody
 str s.fromn(&a[0] a.len)
 out s


opt noerrorshere
str s token=sub.Dialog
 out token

 authToken -> bearer token
typelib WinHttp {662901FC-6951-4854-9EB2-D9A2570F2B2E} 5.1
WinHttp.WinHttpRequest r._create
r.Open("POST" F"https://api.dropboxapi.com/1/oauth2/token?code={token}&grant_type=authorization_code&client_id={appKey}&client_secret={appSecret}")
r.Send()
ARRAY(byte) a=r.ResponseBody
s.fromn(&a[0] a.len)
if(findrx(s "''access_token'': ''(.+?)''" 0 0 token 1)<0) end F"{ERR_FAILED}. {s}"
ret token


#sub Dialog
function'str

 Returns the authorization code, which can be used to attain a bearer token by calling /oauth2/token.

str dd=
 BEGIN DIALOG
 0 "" 0x80C808C8 0x0 0 0 380 300 "Dropbox"
 3 ActiveX 0x54030000 0x0 0 0 380 300 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 4 Edit 0x4C030080 0x200 0 248 96 12 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040307 "*" "" "" ""

str controls = "3 4"
str ax3SHD e4
if(99!=ShowDialog(dd &sub.DlgProc &controls 0 128)) end ERR_FAILED
 if(99!=ShowDialog(dd &sub.DlgProc &controls)) end ERR_FAILED
ret e4


#sub DlgProc v
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	we3.Navigate(F"https://www.dropbox.com/1/oauth2/authorize?client_id={appKey}&response_type=code")
	SetTimer hDlg 1 50 &sub.Timer
	
	case WM_COMMAND ret 1
 
 
#sub Timer
function hDlg uMsg idEvent dwTime

int w=id(3 hDlg)
Htm e
str s
sel idEvent
	case 1 ;;wait for button 'Allow' or 'Sign in'
	e=htm("BUTTON" "^(Allow|Sign in)" "" w "0" 2 0x2)
	if(!e) ret
	KillTimer hDlg idEvent
	s=e.Text
	if(s[0]='A') goto g2
	act hDlg
	SetTimer hDlg 2 50 &sub.Timer
	
	case 2 ;;wait for button 'Allow' and click
	e=htm("BUTTON" "Allow" "" w "0" 2 0x1)
	if(!e) ret
	KillTimer hDlg idEvent
	hid hDlg
	 g2
	e.Click
	SetTimer hDlg 3 50 &sub.Timer
	
	case 3 ;;wait for token and get
	 e=htm("DIV" "auth-code" "" w "0" 22 0x101 0 1)
	e=htm("INPUT" "text" "" w "0" 0 0x501)
	if(!e) ret
	KillTimer hDlg idEvent
	
	s=e.Attribute("value")
	s.setwintext(id(4 hDlg))
	DT_Ok hDlg 99
