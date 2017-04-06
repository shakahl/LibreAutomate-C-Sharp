 /Dialog_Editor
function! $loginURL

 Shows dialog with specified web page.
 Returns: 1 OK, 0 Cancel.
 Use to log in to a website if need when using QM internet functions.
 Actually you can use this function to display any web page, for example to log out.

 EXAMPLE
 for _i 0 2
	 IntGetFile "http://www.quickmacros.com/forum/index.php" _s
	 if(findw(_s "Logout")>=0) break ;;find something that exists only when logged-in
	 if(_i or !IntLoginDialog("http://www.quickmacros.com/forum/ucp.php?mode=login")) end "failed to log in"
 out _s


str dd=
F
 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 521 357 "QM - Login - {loginURL}"
 3 ActiveX 0x54030000 0x0 0 0 522 332 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x0 204 340 48 14 "OK"
 2 Button 0x54030000 0x0 256 340 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x203050A "*" "" "" ""

str controls = "3"
str ax3SHD
ax3SHD=loginURL
if(!ShowDialog(dd 0 &controls)) ret
ret 1
