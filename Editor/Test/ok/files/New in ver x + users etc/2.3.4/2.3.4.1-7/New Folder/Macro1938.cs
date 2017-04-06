str sf.expandpath("$program files$\Foxit Software\Foxit Reader\Foxit Reader.exe")

SHObjectProperties(0 SHOP_FILEPATH @sf L"Digital Signatures")
int w=wait(10 WV win(".exe Properties" "#32770"))
Acc a.Find(w "LISTITEM" "" "class=SysListView32[]id=101" 0x1004 5)
a.Mouse(4)
int w1=wait(10 WV win("Digital Signature Details" "#32770"))
Acc a1.Find(w1 "PUSHBUTTON" "View Certificate" "class=Button[]id=103" 0x1005 5)
a1.DoDefaultAction
int w2=wait(10 WV win("Certificate" "#32770"))
Acc a2.Find(w2 "TEXT" "" "id=106" 0x1004 10)
str s=a2.Value
out s

clo w2
clo w1
clo w
