 /
function# hDlg Acc'a [flags] [accflags] ;;flags: 1 

___EA- dA
int i iFrom htv=id(3 hDlg)

 g1
i=sub.FindInTree(a iFrom)
if i>=0
	SendMessage htv TVM_SELECTITEM TVGN_CARET dA.ar[i].htvi
	ret i

 The rest is to display broken branch, or part of it.
 Noticed it in Word Options dialog and some DW panes.

if(flags&1=0) ret -1
flags=0

 int hc=child(a) ;;in Office gets toplevel
int x y; a.Location(x y)
int hc=child(x y dA.hwnd 32)
if(!hc or hc=dA.hwnd) ret -1

iFrom=dA.ar.len
___EA_ARRAY& r=dA.ar[]
r.a=acc(hc)
r.htvi=TvAdd(htv 0 +LPSTR_TEXTCALLBACK iFrom)

___EA_ENUM e.tv=htv
dA.working=3
acc("" "" hc "" "" accflags &EA_Proc2 &e)
dA.working=0

goto g1

err+ ret -1


#sub FindInTree
function# Acc'a [iFrom]

___EA- dA

str rs rs2 name
int i r r2 x y cx cy x2 y2 cx2 cy2 ies

r=a.Role(rs)
 out rs
a.Location(x y cx cy)
name=a.Name; err
err+ ret -1

 g1
for i iFrom dA.ar.len
	Acc& aa=dA.ar[i].a
	if(aa.elem!=a.elem and !ies) continue ;;makes faster
	r2=aa.Role; if(r2!=r) continue
	if(r*r2=0) aa.Role(rs2); if(rs2!=rs) continue
	aa.Location(x2 y2 cx2 cy2); if(memcmp(&x &x2 16)) continue
	if(aa.Name!=name) continue
	ret i
	err+ continue

 IE bug: object retrieved by acc(mouse) may have incorrect elem, eg in google page in IE 9, elem of buttons is 1 when mouse is in some places (must be 0 in whole button).
if(!ies and WinTest(child(a) "Internet Explorer_Server")) ies=1; goto g1
err+

ret -1

 Other ways to compare Acc:
 IAccIdentity. Unavailable in web pages.
 IUIAutomationElement. Very slow ElementFromIAccessible. In Firefox can be 30 ms.
