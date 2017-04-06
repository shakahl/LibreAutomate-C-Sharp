\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3"
str ax3SHD

 HTML table with questions. Will add radio buttons later.
ax3SHD=
 <style>
 table { border: 2px solid #D1D7DC; border-collapse: collapse; margin: 10px; width: 90%; }
 td,th { border: 1px solid #D1D7DC; border-collapse: collapse; padding-left: 4px; }
 </style>
 <form>
 <table>
 <thead><th>Question</th><th>A</th><th>B</th><th>C</th><th>D</th></thead>
 <!--start-->
 <tr><td>Question 1</td></tr>
 <tr><td>Question 2</td></tr>
 <tr><td>And so on</td></tr>
 </table>
 </form>

 Add radio buttons.
int-- nrows ncols
ncols=4 ;;change if needed
int i j k=find(ax3SHD "<!--start-->")
for i 1 1000000
	k=find(ax3SHD "</tr>" k); if(k<0) break
	str s1=""
	for(j 0 ncols) s1.formata("<td><input type=''radio'' name=''r%i''</input></td>" i)
	ax3SHD.insert(s1 k)
	k+s1.len+5
nrows=i-1
 out ax3SHD
 ret

ax3SHD.setfile("$temp$\x1.htm")
ax3SHD="$temp$\x1.htm"

if(!ShowDialog("dlg_html_questioner" &dlg_html_questioner &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 ActiveX 0x54030000 0x0 0 0 224 112 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

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
	Htm el
	 for each row
	for i 0 nrows
		 find checked column
		for j 0 ncols
			el=htm("INPUT" "" "" hDlg 0 i*ncols+j)
			if(val(el.Attribute("CHECKED"))) break
		if(j=ncols) j=-1
		
		 out checked column
		if(j>=0) out "%i. %i (%c)" i j 'A'+j
		else out "%i. %i (all unchecked)" i j
	
	case IDCANCEL
ret 1
