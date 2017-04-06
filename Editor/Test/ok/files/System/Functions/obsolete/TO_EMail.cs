 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "8 9 10 11 7 24 14 12 16 17 23"
__strt e8 e9 e10 e11 e7 c24Use c14No e12 c16Sen c17Clo c23Don

if(!ShowDialog("" &TO_EMail &controls _hwndqm)) ret

str s sf

if(c24Use=1)
	TO_FlagsFromCheckboxes sf c16Sen 1 c17Clo 2 c23Don 4
	
	s.format("MailMessage %s %s %s %s %s %s %s" e8.S e9.S e10.S e11.S e7.S e12.S sf)
	sub_to.Trim s " '''' '''' '''' '''' '''' 0"
else
	str* p=&e8; int i
	for(i 0 4) if(matchw(p[i] "(*)")) out "Note: Variables cannot be used in this field, unless 'Use MAPI' is checked."
	sf.format("''mailto:%s&cc=%s&bcc=%s&subject=%s''" e8 e9 e10 e11.escape(9))
	sf.replacerx("&\w+=(?=(&|''))")
	i=sf.findreplace("&" "?" 4)>0
	if(e7.len)
		e7.S
		s.format("_s=%s; _s.escape(9)[]run _s.from(%s ''%cbody='' _s)" e7 sf iif(i '&' '?'))
	else s.from("run " sf)

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80848 0x100 0 0 319 252 "New email message (use mailto or MAPI)"
 3 Static 0x54020000 0x4 4 6 14 10 "To"
 8 Edit 0x54030080 0x204 42 4 274 14 ""
 4 Static 0x54020000 0x4 4 22 34 10 "Cc"
 9 Edit 0x54030080 0x204 42 20 274 14 ""
 5 Static 0x54020000 0x4 4 38 34 10 "Bcc"
 10 Edit 0x54030080 0x204 42 36 274 14 ""
 6 Static 0x54020000 0x4 4 58 34 10 "Subject"
 11 Edit 0x54030080 0x204 42 56 274 14 ""
 7 Edit 0x54231044 0x204 4 76 312 78 ""
 24 Button 0x54012003 0x0 4 156 142 13 "Use MAPI (if unchecked, use mailto)"
 15 Button 0x5C032000 0x4 4 172 36 14 "Attach..."
 14 Button 0x5C012003 0x0 4 188 36 13 "No SF" "Let the button give me normal path, not special folder name"
 12 Edit 0x5C2310C4 0x204 42 172 274 29 ""
 16 Button 0x5C012003 0x4 4 208 38 13 "Send"
 17 Button 0x5C012003 0x4 50 208 96 13 "Close warning message"
 23 Button 0x5C012003 0x0 152 208 48 13 "Don't wait"
 20 Button 0x5C032000 0x4 274 208 40 14 "Profile..."
 19 QM_DlgInfo 0x54000000 0x20000 146 236 170 13 "Creates new message in default email program."
 1 Button 0x54030001 0x4 4 234 48 14 "OK"
 2 Button 0x54030000 0x4 54 234 48 14 "Cancel"
 22 Button 0x54032000 0x4 104 234 16 14 "?"
 18 Static 0x54000010 0x20004 0 226 330 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\email.ico" "12" 1)) ret wParam
sel message
	case WM_INITDIALOG
	sub_to.SetUserFont hDlg "7"
	SendDlgItemMessage(hDlg 7 EM_LIMITTEXT 4000 0)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 24 TO_Enable hDlg "15 14 12 16 17 23 20" but(lParam); if(but(lParam)) goto g1
	case 15
	if(sub_to.FileDialog(hDlg 0 "attachdir" "" "" "" sf))
		s.getwintext(id(12 hDlg)); s.addline(sf 1)
		TO_SetText s hDlg 12
	case 16
	 g1
	TO_Enable hDlg "17" but(16 hDlg)
	case 20
	str spr spa
	if(inp(spr "Default Outlook profile:"))
		_i=inp(spa "Password (optional):")
		MapiProfile 0 spr spa _i
	case 22 QmHelp iif(but(24 hDlg) "MailMessage" "IDP_RUN")
ret 1

#opt nowarnings 1
