\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
if(getopt(nthreads)>1) ret
 g1
 ______________________________________________

 This is your About dialog. Displays short information about your macros (product name, version, copyright, etc).
 A customer enters here his/her registration code to unlock your product for use beyond the evaluation period.
 This dialog is automatically shown if the evaluation period is expired. To unlock the product while the
 evaluation period is not expired, let the customer run this function.

 Change the name of this function so that it would be unique.
 Encrypt this function and distribute together with your macros.

 This is your unique data that you must change.
lpstr sm_registry_key_regcode="Software\Company\Product" ;;Change Company\Product to match you company (or name) and product name. This key is not secret. It will be created in HKEY_CURRENT_USER hive. Use the same value in ..._CanRun.
lpstr sm_website_url="http://www.smmacros.com" ;;change this to your product's website url or mailto:email
lpstr sm_support_url="mailto:support@smmacros.com?subject=SM Macros" ;;change this to your product's support url or mailto:email
lpstr sm_buynow_url="mailto:sales@smmacros.com?subject=SM%20Macros" ;;change this to your email or registration page url
lpstr sm_product_name="SM Macros" ;;change this
lpstr sm_about="SM Macros, version 1.0.0[][]Copyright 2008 ...[][]This product is shareware. To use it beyond the evaluation period, you must have a license for this product. You also must have Quick Macros license." ;;change this
 Also, change SM_CanRun (in 1 place below) and SM_About (in 1 place below) to the names of the renamed functions.

 ______________________________________________

if(wParam) goto messages2
str controls = "0 3 11 4 12"
str d0 e3 e11 e4 e12

d0.from("About " sm_product_name)
if !message
	int days_left
	SM_CanRun days_left
	if(days_left>0) e11.format("UNREGISTERED. %i days left." days_left-1)
	else if(days_left<0) ;;registered
		rget e11 "regcode" sm_registry_key_regcode
		e11.get(e11 17); e11.escape(8); e11-"This product is licensed to:[][]"
if(days_left=0) e11="UNREGISTERED. The evaluation period is expired."
if(days_left>=0) e11+"[][]To register, enter your registration code for this product in the field below and click Unlock. To get the registration code, click Buy Now.[][]Note: Don't confuse this product with Quick Macros. Don't use Quick Macros registration code here."
e3=sm_about
GetVolumeInformation("c:\" 0 0 &_i 0 0 0 0); e12=_i&0xffff
 GetUserComputer 0 e12

str sm_dialog_definition=
 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 265 215 "About"
 2 Button 0x54030001 0x4 204 198 54 14 "Close"
 3 Edit 0x54030844 0x4 10 6 248 62 ""
 11 Edit 0x54030844 0x4 10 80 246 68 ""
 4 Edit 0x54030080 0x204 10 150 246 14 ""
 7 Button 0x54032000 0x4 10 168 54 14 "Buy Now"
 5 Button 0x54032000 0x4 68 168 54 14 "Unlock"
 6 Static 0x54000000 0x0 156 170 48 12 "Computer ID:"
 12 Edit 0x54030844 0x20000 206 170 50 12 ""
 9 Button 0x54032000 0x4 10 198 54 14 "Website"
 10 Button 0x54032000 0x4 68 198 54 14 "Support (email)"
 8 Button 0x54020007 0x4 4 68 258 122 "License"
 END DIALOG
 DIALOG EDITOR: "" 0x2010805 "*" ""

sm_dialog_definition.replacerx("^" " " 8)
if(!ShowDialog(sm_dialog_definition &SM_About &controls 0 2)) ret

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto g1 ;;needs strings
ret
 messages2
sel wParam
	case 5 ;;unlock
	str s.getwintext(id(4 hDlg)); s.trim
	if(s.len)
		if(!rset(s "regcode" sm_registry_key_regcode)) mes- "Failed. Try to log on as an administrator." sm_product_name "x"
	case 7 run sm_buynow_url
	case 9 run sm_website_url
	case 10 run sm_support_url
	case IDCANCEL
ret 1
