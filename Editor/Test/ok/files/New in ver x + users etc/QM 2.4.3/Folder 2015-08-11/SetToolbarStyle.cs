 /
function# $name $layout style [flags] ;;flags: 0 set, 1 add, 2 remove, 3 toggle

 Changes user-defined toolbar right-click menu settings (style).
 Returns new style flags.

 name - toolbar name (full, case insensitive) or +id (integer). Also can be QM item path or GUID string.
 layout - from Options -> General -> Layout of toolbars. Use "" for current layout.
 style - style flags:
   1 follow owner
   2 text
   4 activate owner
   8 shrink
   16 equal buttons
   32 no sizing border
   64 tooltips
   128 vertical
   0x100 top-right
   0x200 bottom-left
   0x300 bottom-right
   0x400 auto-select
   0x1000 cannot hide
   0x2000 quick icons
   0x4000 1-pixel border
   0x8000 3d buttons
   0x10000 drop-open file
   0x20000 full-screen hide
   0x40000 topmost

 REMARKS
 If the toolbar is running, restarts it.

 EXAMPLE
 SetToolbarStyle("QM toolbar" "" 8 3) ;;toggles "auto shrink"


opt noerrorshere

int hOwner hTB=win(name "QM_toolbar" "qm" 2)
if(hTB) hOwner=GetToolbarOwner(hTB); clo hTB; rep() 0.01; if(!IsWindow(hTB)) 0.01; break

if(empty(layout)) layout="tb"; else layout=_s.from("tb " layout)
int _1 _2 _3 _style
_qmfile.SettingGetB(name layout &_1 16)
sel(flags&3) case 0 _style=style; case 1 _style|style; case 2 _style~style; case 3 _style^style
_qmfile.SettingAddB(name layout &_1 16)

if(hTB) mac name hOwner

ret _style
