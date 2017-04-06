function $appKey $appSecret

 Authorizes your Dropbox application (actually the caller macro) to use Dropbox API with user's Dropbox account.

 appKey - from <link>https://www.dropbox.com/developers/apps</link> -> your app -> App key.
 appSecret - from <link>https://www.dropbox.com/developers/apps</link> -> your app -> App secret.

 REMARKS
 Opens https://www.dropbox.com/1/oauth2/authorize?client_id={appKey}... in a hidden web browser control, clicks Allow and gets token from next page.
 Shows Dropbox sign-in page, let the user enter Dropbox email and password.
 Error if fails, for example if the user closes the email/password dialog.

 To get app key and secret:
 <link>https://www.dropbox.com/developers/apps</link>
 Click 'Create app' ...
 Then you'll find these values in app settings.

 There is another (faster) way to authorize, but it can be used only if both the app and the account is yours:
   Go to <link>https://www.dropbox.com/developers/apps</link> -> your app, click Generate button under "Generated access token", copy the token.
   Assign it to the member variable 'token' instead of calling Authorize().
   Example: Dropbox x; x.token="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx".


opt noerrorshere

str _token=sub.Dialog
 out token

 authToken -> bearer token
WinHttp.WinHttpRequest r._create
r.Open("POST" F"https://api.dropboxapi.com/1/oauth2/token?code={_token}&grant_type=authorization_code&client_id={appKey}&client_secret={appSecret}")
r.Send()
ARRAY(byte) a=r.ResponseBody
str s.fromn(&a[0] a.len)
if(findrx(s "''access_token'': ''(.+?)''" 0 0 token 1)<0) end F"{ERR_FAILED}. {s}"


#sub Dialog
function'str

 Returns the authorization code, which can be used to attain a bearer token by calling /oauth2/token.

str dd=
 BEGIN DIALOG
 0 "" 0x80C808C8 0x0 0 0 380 342 "Dropbox"
 3 ActiveX 0x54030000 0x0 0 0 380 342 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 4 Edit 0x4C030080 0x200 0 248 96 12 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

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
	rep() opt waitmsg 1; 0.01; if(!we3.Busy) break ;;older IE would not create "Internet Explorer_Server" until Navigate() complete; then error "window not found" in htm()
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
	if(!e)
		e=htm("H1" "Error (*)" "" w "0" 0 0x1 3)
		if(e)
			KillTimer hDlg 1
			act hDlg
		ret
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
