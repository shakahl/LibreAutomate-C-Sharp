 /
function# hwnd str&sf

 Shows menu of special folders and gets selected string.
 Returns 1 if an item selected, 0 if not.

 hwnd - menu owner window handle.
 sf - variable that receives selected string.


MenuPopup pm
str sm s ss sa; int i j n

 add SF
INTLPSTR* asf=__SfMap(&n) ;;CSIDL/name map
for i 0 n
	INTLPSTR& r=asf[i]
	if(i and r.i=asf[i-1].i) continue ;;alias
	sm+F"{i+1} {r.s}[]" ;;item
	sel(r.i) case 7 sm+"|[]"; case [0x37,0xF5] sm+"-[]" ;;separator

 add env var
sm+">Environment variables[]"
word* se0(GetEnvironmentStringsW) se(se0)
for i 1000000 1000000000
	if(empty(se)) break
	sa.ansi(se)
	j=findrx(sa "=(\w:|[\\$%])[^;]+$") ;;file path?
	if j>0
		s.left(sa j); j+1
		ss=sa+j; ss.LimitLen(50 2)
		sm+F"{i} {s}[9]{ss}[]"
	se+(lstrlenW(se)+1)*2
FreeEnvironmentStringsW se0
sm+"<"

 show menu
pm.AddItems(sm)
i=pm.Show(hwnd)
if(!&sf) ret i!0
if(!i) sf.all; ret

 get text
pm.GetItemText(i s ss)
if(i<1000000) sf=F"${s}$" ;;SF
else sf=F"%{s}%" ;;EV

ret 1

 note: this is public function, before QM 2.3.4 exported from qm.exe.
