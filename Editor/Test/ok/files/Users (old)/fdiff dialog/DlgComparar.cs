 \Macro130
function# hDlg message wParam lParam
if(hDlg) goto messages

 This function allows to execute code whenever some event
 (dialog created, button clicked, etc) occurs in dialog.
 The code must follow appropriate case statements.
 To add case statements for various messages (events), you
 can use the Events button in the Dialog Editor. Read more in Help.

 BEGIN DIALOG
 0 "" 0x10CA0A44 0x110 0 0 419 287 "Comparar"
 1 Button 0x54030001 0x4 160 268 48 14 "OK"
 2 Button 0x54030000 0x4 212 268 48 14 "Cancel"
 3 Edit 0x543311C4 0x204 4 10 204 198 ""
 4 Edit 0x543311C4 0x204 212 10 204 198 ""
 5 Button 0x54013003 0x4 186 228 50 18 "Solo diferencias"
 6 Edit 0x54030081 0x204 162 248 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010700 "" ""

ret
 messages
sel message
	case WM_DROPFILES
	int h=child(mouse)
	sel(GetWinId(h)) case [3,4] case else ret
	str s.fix(DragQueryFile(wParam 0 s.all(MAX_PATH) MAX_PATH))
	_s.getfile(s)
	 anadir_numeros_linea(_s)
	_s.setwintext(h)
	 iff(tex1)
		 out 1
		 str tex11.getfile(tex1)
		  anadir_numeros_linea(tex11)
		 tex11.setwintext(id(3 "Comparar"))
	 str tex2.getwintext(id(4 "Comparar"))
	 iff(tex2)
		 out 2
		 str tex22.getfile(tex2)
		  anadir_numeros_linea(tex22)
		 tex22.setwintext(id(4 "Comparar"))
	str+ texto1.getwintext(id(3 "Comparar"))
	str+ texto2.getwintext(id(4 "Comparar"))
	if(texto1=texto2) _s="IDÉNTICOS"
	else _s="DIFERENTES"
	_s.setwintext(id(6 "Comparar"))
	case WM_INITDIALOG DT_Init(hDlg lParam); ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
int ctrlid=wParam&0xFFFF; message=wParam>>16
sel wParam
	case 5
	int h1=id(3 hDlg)
	int h2=id(4 hDlg)
	int i
	int minlen=iif(texto1.len<texto2.len texto1.len texto2.len)
	for i 0 minlen
		if(texto1[i]!=texto2[i])
			SetFocus h1
			SendMessage h1 EM_SETSEL i i+1
			SendMessage h1 EM_SCROLLCARET 0 0
			SetFocus h2
			SendMessage h2 EM_SETSEL i i+1
			SendMessage h2 EM_SCROLLCARET 0 0
			break

	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
