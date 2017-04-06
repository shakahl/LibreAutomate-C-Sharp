 /
function# unused1 unused2 action UDTRIGGER&p

int vk mod fl hDlg
int+ __kt2hwnd

sel action
	case 1 ;;create and initialize property page
	hDlg=ShowDialog("" 0 0 p.hwnd 1 WS_CHILD)
	SendMessage(id(3 hDlg) HKM_SETRULES 0 0)
	if(KT2_Parse(p.tdata mod vk fl 1))
		if(fl&1) but+ id(5 hDlg)
		if(fl&2) but+ id(6 hDlg)
		SendMessage(id(3 hDlg) HKM_SETHOTKEY mod<<8|vk 0)
	ret hDlg
	
	case 2 ;;collect property page data and format trigger-string
	vk=SendMessage(id(3 p.hwnd) HKM_GETHOTKEY 0 0)
	if(!vk) ret 1
	QmKeyCodeFromVK(vk&255 &p.tdata)
	mod=vk>>8
	if(mod&2) _s+"C"
	if(mod&1) _s+"S"
	if(mod&4) _s+"A"
	if(but(id(5 p.hwnd))) _s+"W"
	p.tdata-_s
	if(but(id(6 p.hwnd))) fl|2
	if(fl) p.tdata.formata(" 0x%X" fl)
	
	case 3 ;;validate trigger-string
	if(KT2_Parse(p.tdata mod vk fl 0)) ret 1
	
	case 4 ;;[re]create triggers
	if(!IsWindow(__kt2hwnd))
		 launch trigger engine
		__kt2hwnd=0
		mac "KT2_Main"
		opt waitmsg 1
		wait 30 V __kt2hwnd
	SendMessage __kt2hwnd WM_APP 0 &p ;;let the trigger engine to [re]create triggers
	
	case 5 ;;provide icon
	ret GetIcon("keyboard.ico")
	
	case 6 ;;provide help
	str s1("Keyboard Triggers 2 Help") s2
	ShowText s1 s2.getmacro(s1)
	
	case [7,8] ;;QM or an item enabled or disabled
	ret 1 ;;returns 1 because needs to recreate triggers

 BEGIN DIALOG
 0 "" 0x0000004C 0x10000 0 0 189 119 ""
 4 Static 0x54000000 0x0 6 8 48 12 "hotkey:"
 3 msctls_hotkey32 0x54000000 0x200 4 22 138 14 ""
 5 Button 0x54012003 0x0 4 40 44 12 "Win"
 6 Button 0x54012003 0x0 4 102 92 12 "Don't release modifiers"
 END DIALOG
 DIALOG EDITOR: "" 0x2010801 "" ""
