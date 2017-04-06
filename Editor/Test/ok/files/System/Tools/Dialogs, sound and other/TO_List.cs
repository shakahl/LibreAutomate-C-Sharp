 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 20 6 8 9 13 14 18"
__strt e3Ite c20Use e6Des e8Tit c9Don e13Tim e14Def cb18Res

cb18Res="&Use sel[]Store selected index or ID into variable"

if(!ShowDialog("" &TO_List &controls _hwndqm)) ret

int i f isVar nl=numlines(e3Ite.rtrim("[]"))
ARRAY(str) a

if(c9Don=1) f|1
if(c20Use=1) f|2; a=e3Ite

str s=F"ListDialog({e3Ite.S(`` isVar)} {e6Des.S} {e8Tit.S} 0x{f} 0 0 0 {e13Tim.N} {e14Def.N})"
sub_to.Trim s " '''' '''' 0x0 0 0 0 0 0"

sel val(cb18Res)
	case 0
	s-"sel("; s+")"
	if(a.len and !isVar) for(i 0 a.len) s.formata("[][9]case %i" val(a[i]))
	else for(i 0 iif(isVar||!nl 3 nl)) s.formata("[][9]case %i" i+1)
	s+"[][9]case else ret"
	
	case 1 s-cb18Res.VD("int iSel=")

 sub_to.TestDialog s
InsertStatement s

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 239 188 "List box"
 4 Static 0x54020000 0x4 4 4 40 13 "List items"
 3 Edit 0x54231044 0x204 46 4 190 64 "Ite" "Example:[]Red[]Green[]Blue"
 20 Button 0x54012003 0x4 4 20 40 21 "Use       item ID" "Each line begins with a number (item ID) that will be returned instead of line index.[]Example:[]1 Red[]2 Green[]4 Blue"
 5 Static 0x54020000 0x4 4 74 40 12 "Description"
 6 Edit 0x54231044 0x204 46 72 190 20 "Des"
 7 Static 0x54020000 0x4 4 98 40 12 "Title"
 8 Edit 0x54030080 0x204 46 96 190 14 "Tit"
 9 Button 0x54012003 0x0 4 114 64 14 "Don't activate" "Don't activate list box window"
 12 Static 0x54020000 0x4 88 116 40 13 "Close after"
 13 Edit 0x54030080 0x204 130 114 26 14 "Tim" "Timeout.[]If specified, the list box will close self after this time.[]"
 21 Static 0x54000000 0x0 160 116 48 13 "s, and return"
 14 Edit 0x54030080 0x204 210 114 26 14 "Def" "A value to return on timeout.[]Usually 1-based item index or item ID.[]Default 0."
 17 Static 0x54020000 0x4 4 144 64 12 "Result processing"
 18 ComboBox 0x54230243 0x4 70 142 166 213 "Res"
 1 Button 0x54030001 0x4 4 170 48 14 "OK"
 2 Button 0x54030000 0x4 54 170 48 14 "Cancel"
 16 Button 0x54032000 0x4 104 170 16 14 "?"
 10 Button 0x54032000 0x0 180 170 56 14 "More options"
 19 Static 0x54000010 0x20004 0 162 256 1 ""
 15 Static 0x54000010 0x20004 2 134 234 1 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "*" "" "" ""

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "0[]$qm$\list.ico" "" 1)) ret wParam
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 10 mes "This dialog creates basic code to call function ListDialog. The function also allows you to set dialog position, size, owner window and item images. Read ListBox help and edit the inserted code." "" "i"
	case 16 QmHelp "ListDialog"
ret 1
