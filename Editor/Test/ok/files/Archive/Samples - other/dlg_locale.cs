\Dialog_Editor

 Converts number, currency and date/time format from your locale to US locale and backwards.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 6 7"
str e4 e6 cb7
cb7="&Number[]Currency[]Date and/or Time"
if(!ShowDialog("dlg_locale" &dlg_locale &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 157 58 "Locale Converter"
 3 Static 0x54000000 0x0 2 24 48 12 "In your locale"
 4 Edit 0x54030080 0x200 52 22 102 14 ""
 5 Static 0x54000000 0x0 2 42 48 12 "In US locale"
 6 Edit 0x54030080 0x200 52 40 102 14 ""
 7 ComboBox 0x54230243 0x0 52 4 102 213 ""
 8 Static 0x54000000 0x0 2 6 48 12 "What"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
str s
int hr
BSTR b
double d
CURRENCY c
DATE da
sel wParam
	case CBN_SELENDOK<<16|7
	_s.setwintext(id(4 hDlg)); _s.setwintext(id(6 hDlg))
	
	case [EN_CHANGE<<16|4,EN_CHANGE<<16|6]
	if(GetFocus!lParam) ret
	int tous(wParam&0xffff=4) l1(iif(tous LOCALE_USER_DEFAULT 0x409)) l2(iif(tous 0x409 LOCALE_USER_DEFAULT))
	s.getwintext(lParam)
	if(s.len)
		b=s
		sel CB_SelectedItem(id(7 hDlg))
			case 0 hr=VarR8FromStr(b l1 0 &d); if(!hr) hr=VarBstrFromR8(d l2 0 &b)
			case 1 hr=VarCyFromStr(b l1 0 &c); if(!hr) hr=VarBstrFromCy(c l2 0 &b)
			case 2 hr=VarDateFromStr(b l1 0 &da); if(!hr) hr=VarBstrFromDate(da l2 0 &b)
		if(hr) s.dllerror("" "" hr); else s=b
	s.setwintext(id(iif(tous 6 4) hDlg))
	
	case IDOK
	case IDCANCEL
ret 1
