 /dialog_QM_Tools
str s si; int i j h n

 get windows to array
ARRAY(int) a av
GetMainWindows a 4
 get all visible and append to a
win "" "" "" 0 0 0 av
n=a.len
for j 0 av.len
	h=av[j]
	for(i 0 n) if(h=a[i]) break
	if(i<n) continue
	sel WinTest(h "Windows.UI.Core.CoreWindow[]ApplicationFrameWindow[]QM_Toolbar[]SysShadow[]tooltips_class32[]Internet Explorer_Hidden[]EdgeUiInput*[]DesktopWallpaperManager")
		case 0
		case 1 if(sub_sys.GetWindowsStoreAppHost(h)) continue
		case else continue
	if(h=m_hparent) continue
	a[]=h

 create CSV for drop-down list
ICsv x._create
__ImageList il.Create; si=il
x.AddRowSA(0 2 &s)
for i 0 a.len
	h=a[i]
	s.getwintext(h); if(!s.len) s.getwinclass(h); s-"+"
	int hi=GetWindowIcon(h); if(hi) si=ImageList_ReplaceIcon(il -1 hi); DestroyIcon hi; else si=""
	x.AddRowSA(-1 2 &s)

 show drop-down list
if(ShowDropdownList(x i 0 1 mw_heW)&1=0) ret

h=a[i]
if(!RecGetWindowName(h &s)) ret
mw_comments.all
mw_captW=h; sub_to.SetTextNoNotify mw_heW s
mw_captC=0; sub_to.SetTextNoNotify mw_heC ""
_WinSelect(1)
SendMessageW m_hparent __TWN_WINDOWCHANGED h mw_heW
