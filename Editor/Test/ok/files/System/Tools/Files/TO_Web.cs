 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str dd=
F
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 320 224 "Open web page"
 5 QM_DlgInfo 0x54000000 0x20000 0 0 322 20 "<>Opens web page in Internet Explorer. If only URL specified - in default web browser, like <help>run</help>. To wait in other browsers use <fa ''{&sub.Info}''>other functions</fa>."
 3 Static 0x54020000 0x4 4 26 18 12 "URL"
 4 ComboBox 0x54230242 0x4 24 24 292 213 "Url"
 28 Button 0x54032000 0x4 24 38 42 14 "Browse..."
 33 Button 0x54032000 0x0 66 38 42 14 "Favorites"
 29 Button 0x54032000 0x4 108 38 18 14 "SF" "Special folders"
 30 Button 0x54012003 0x0 144 40 38 12 "No SF" "Let the button give me normal path, not special folder name"
 8 Static 0x54000000 0x0 4 62 58 12 "Open in window"
 10 ComboBox 0x54230243 0x4 64 60 118 213 "Win"
 9 QM_Tools 0x44030000 0x10000 4 76 312 14 "1 0x1C1"
 11 ComboBox 0x54230243 0x4 10 112 122 213 "Wai"
 18 Static 0x44020000 0x4 10 130 38 13 "Final URL"
 12 Edit 0x44030080 0x204 50 128 194 14 "Fin" "When the page is loaded, URL must contain this string"
 17 Button 0x44012003 0x4 246 128 36 13 "Use *" "The specified final URL is full or with wildcard characters *?"
 16 Button 0x44032000 0x4 284 128 26 15 "=URL" "Final URL must be same as URL"
 19 Static 0x44020000 0x4 222 112 58 13 "Max wait time, s"
 20 Edit 0x44032080 0x204 282 110 28 14 "Wai" "Default infinite"
 6 Static 0x54000000 0x0 4 160 18 12 "Get"
 14 ListBox 0x54230109 0x200 24 158 108 32 "Get" "Select one or several"
 1 Button 0x54030001 0x4 4 204 48 14 "OK"
 2 Button 0x54030000 0x4 54 204 48 14 "Cancel"
 15 Button 0x54032000 0x4 104 204 18 14 "?"
 7 Button 0x54020007 0x4 4 98 312 50 "Wait, check final URL"
 13 Static 0x54000010 0x20000 0 196 348 2 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

str controls = "4 30 10 9 11 12 17 20 14"
__strt cb4Url c30No cb10Win qmt9 cb11Wai e12Fin c17Use e20Wai lb14Get
cb4Url="&http://[]https://[]mailto:[]javascript:[]Back[]Forward[]Stop[]Home[]Refresh[]Search[]Quit[]To apply other options (wait, etc) on current page, leave this field empty.[]To easily insert an url (link, Internet shortcut), drag and drop it onto the editor text."
cb11Wai="&Don't wait[]Wait while page is loading[]Wait, and check final URL[]Wait for final URL"
cb10Win="&Existing or new[]Existing, or error[]New[]New Internet Explorer process[]Specified"
lb14Get="Final URL[]Window handle[]IWebBrowser2"

if(!ShowDialog(dd &TO_Web &controls _hwndqm)) ret

int f cb1(val(cb11Wai)) cb2(val(cb10Win))
str s w(0) p1(" ") p2 winVar varURL varHWND
__strt vd fs

cb4Url.CbItem
qmt9.Win(winVar "''''")

sel(cb2) case 1 f|2; case 2 f|4; case 3 f|8; case 4 w=winVar
sel(cb1) case 0 e20Wai=""; case [1,2] f|1
if(cb1<2) e12Fin=""; else if(c17Use=1) f|16
fs.Flags(f); if(e20Wai.len) fs=F"MakeInt({fs} {e20Wai})"

if(lb14Get[0]='1') s=vd.VD("str url[]" varURL); else varURL=0
if(lb14Get[1]='1') s+vd.VD("int w[]" varHWND); else varHWND=0
if(lb14Get[2]='1') s+vd.VD("SHDocVw.IWebBrowser2 wb="); p1="("; p2=")"

s+F"web{p1}{cb4Url.S} {fs} {w} {e12Fin.S} {varURL} {varHWND}{p2}"
sub_to.Trim s " 0 0 '''' 0 0"

 sub_to.TestDialog s
InsertStatement s

ret
 messages
if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\web.ico" "1001" 1)) ret wParam
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 28 sub_to.FileDialog hDlg 4 "urldir" "$favorites$" "Internet shortcuts[]*.url[]All files[]*.*[]" "url"
	case 33 sub_to.File_FileMenu hDlg 4 2
	case 29 sub_to.File_SF hDlg 4 "\"
	case 16 TO_SetText "*" hDlg 12
	
	case CBN_SELENDOK<<16|11
	_i=TO_Selected(lParam)
	TO_Show hDlg "19 20" _i!0
	TO_Show hDlg "18 12 16 17" _i>=2
	
	case CBN_SELENDOK<<16|10
	TO_Show hDlg "9" TO_Selected(lParam)=4
	
	case 15 QmHelp "IDP_WEB"
ret 1

#opt nowarnings 1


#sub Info
function[c] $param
_s=
 The 'Open web page' dialog creates code for <help>web</help>. It supports "wait" and other features only in Internet Explorer, IE-based browsers and browser controls.
;
 Does not support Firefox, Chrome and Opera. To wait in these browsers can be used functions from <link "http://www.quickmacros.com/forum/viewtopic.php?f=2&t=4538">forum</link>. However they are not so reliable, eg may stop working in a new browser version, or may need editing for non-English browser versions.
;
 Also can wait for an accessible object that must be in the web page when it is loaded. To create code, use dialog 'Find accessible object'. Capture the object and specify a wait time. It works in any browser that supports and has enabled <help #IDP_ACCESSIBLE>accessible objects in web page</help>.
QmHelp _s 0 6
