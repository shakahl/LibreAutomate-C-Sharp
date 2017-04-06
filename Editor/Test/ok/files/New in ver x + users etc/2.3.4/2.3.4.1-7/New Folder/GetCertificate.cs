 /Macro1937
function! $_file str&sCA

spe -1
str sf.expandpath(_file)
str fn.getfilename(_file)

if(!SHObjectProperties(0 SHOP_FILEPATH @sf L"Digital Signatures")) ret
int w=wait(30 WA win(F"{fn}.exe Properties" "#32770" "" 2))
Acc a.Find(w "LISTITEM" "" "class=SysListView32[]id=101" 0x1004)
a.Mouse(4)
int w1=wait(30 WA win("Digital Signature Details" "#32770" w 32))
Acc a1.Find(w1 "PUSHBUTTON" "View Certificate" "class=Button[]id=103" 0x1005 5)
a1.DoDefaultAction
int w2=wait(30 WA win("Certificate" "#32770" w1 32))
Acc a2.Find(w2 "TEXT" "" "id=106" 0x1004)
sCA=a2.Value

int R=1
err+
if(w2) clo w2
if(w1) clo w1
if(w) clo w
ret R
