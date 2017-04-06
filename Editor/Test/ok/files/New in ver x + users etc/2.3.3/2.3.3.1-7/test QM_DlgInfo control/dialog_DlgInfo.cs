\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
if(!ShowDialog("dialog_DlgInfo" &dialog_DlgInfo 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 208 239 "Dialog"
 3 QM_DlgInfo 0x54000000 0x20000 4 4 150 230 "<>Wan be text <b>bold</b> <help>ShowDialog</help> some more text."
 4 Button 0x54030000 0x4 158 2 48 14 "setwintext"
 END DIALOG
 DIALOG EDITOR: "" 0x2030306 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int c=id(3 hDlg)
	SetProp c "wndproc" SetWindowLongW(c GWL_WNDPROC &WndProc_SubclassDlgInfo)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	
ret
 messages2
sel wParam
	case 4
	 _s="str"; _s.setwintext(id(3 hDlg))
	goto g1
ret 1

 g1
str s=
 <><link>http://www.quickmacros.com</link>
 <link>notepad.exe</link>
 <link "http://www.quickmacros.com">quickmacros</link>
 <link "notepad.exe">notepad</link>
 <link "notepad.exe /$desktop$\test.txt">run notepad with command line parameters</link>
 <google>quick macros</google>
 <google "quick macros">google qm</google>
 <macro>Macro1671</macro>
 <macro "Macro1671 /param">macro</macro>
 <open>dialog_DlgInfo</open>
 <open "dialog_DlgInfo /10">open, go to pos 10</open>
 <open "dialog_DlgInfo /L10">open, go to line 10</open>
 <help>ShowDialog</help>
 <help "::/User/IDH_DIALOG_EDITOR.html#A9">dialog procedure</help>
 <tip "E_IF">display a tip from $qm$\tips.txt</tip>
 <tip "#ShowText">display a macro or function (only help section) in tips</tip>
s.setwintext(id(3 hDlg))
