\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD

_s=
 <html>
 <style> BODY { border: 0px; margin: 0px 0px 0px 0px; overflow: auto; background-color: #%02x%02x%02x; } </style>
 <body>
 <form target="PayPal" action="https://www.paypal.com/cgi-bin/webscr" method="post">
 <input type="hidden" name="cmd" value="_xclick">
 <input type="hidden" name="business" value="My PAYPAL eMAIL">
 <input type="hidden" name="item_name" value="PRODUCT 1">
 <input type="hidden" name="item_number" value="">
 <input type="hidden" name="amount" value="4.99">
 <input type="hidden" name="currency_code" value="GBP">
 <input type="hidden" name="shipping" value="0">
 <input type="hidden" name="shipping2" value="0">
 <input type="hidden" name="handling" value="0">
 <input type="hidden" name="image_url" value="">
 <input type="hidden" name="return" value="">
 <input type="hidden" name="cancel_return" value="">
 <input type="hidden" name="undefined_quantity" value="0">
 <input type="hidden" name="receiver_email" value="MY PAYPAL eMAIL">
 <input type="hidden" name="no_shipping" value="1">
 <input type="hidden" name="no_note" value="0">
 <input type="image" name="submit" src="http://images.paypal.com/images/x-click-butcc.gif" alt="Make payments with PayPal, it's fast, free, and secure!">
 </form>
 </body>
 </html>

int R G B; ColorToRGB GetSysColor(COLOR_BTNFACE) R G B; ax3SHD.format(_s R G B)

if(!ShowDialog("Dialog42" &Dialog42 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 172 105 "Dialog"
 3 ActiveX 0x54030000 0x0 62 32 54 32 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x0 40 88 48 14 "OK"
 2 Button 0x54030000 0x0 92 88 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x203000E "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
