 Creates HTML for a type-info list in features.html.

 Before:
 1. Copy this macro to empty.qml to avoid adding nonsystem functions.
 2. Assign a hot key trigger, eg F12.

 Usage:
 1. Type category or class name in last line of this macro. It will be title of the list in the web page. Then type . to show its members.
 2. Press hotkey to run this macro. It Creates HTML for the list.
 3. Paste in the web page, in code view.

byte noHead=1 ;;don't add <td>title. Use when updating existing list in web page.

out
int h=id(2205 _hwndqm)
if(hid(h)) end "typeinfo popup must be visible"

int i j n
LVITEM li
str s ss.all(300) sss
ARRAY(STRINT) a

 get item text and image index
n=SendMessage(h LVM_GETITEMCOUNT 0 0)
a.create(n)
for i 0 n
	li.iItem=i
	li.mask=LVIF_TEXT|LVIF_IMAGE
	li.pszText=ss; li.cchTextMax=300
	if(!SendMessage(h LVM_GETITEM 0 &li)) end "error"
	a[i].i=li.iImage; a[i].s=li.pszText

 get category or class name (last line, first word)
int hwnd=GetQmCodeEditor; int lens=SendMessage(hwnd SCI.SCI_GETTEXTLENGTH 0 0); sss.fix(SendMessage(hwnd SCI.SCI_GETTEXT lens+1 sss.all(lens))) ;;same as SciGetText
sss.getl(sss numlines(sss)-1)
sss.gett(sss 0)

 format
if(noHead) s.from("<ul>"); else s.from("<td>" sss "[]<ul>")
for i 0 n
	j=a[i].i
	if(j<10) j+'0'; else j+'a'-10
	s.formata("<li id=''i%c''>%s</li>" j a[i].s)
 if(n<a.len) s.formata("<li id=''ig''>... (%i items)</li>" a.len)
s+"</ul></td>[]"
if(!noHead) s+"[]"

s.setclip
out "Stored to clipboard:"
out s
key ZSHX

WindowText A ;;this is for classes. Change class name, type A. ...
#ret
