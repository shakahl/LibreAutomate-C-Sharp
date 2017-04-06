 /
function# $name [$layout]

 Gets user-defined toolbar right-click menu settings.

 name - toolbar name (full, case insensitive) or +id (integer). Also can be QM item path or GUID string.
 layout - from Options -> General -> Layout of toolbars.

 Returns these flags:
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

 EXAMPLE
 if(GetToolbarStyle("QM toolbar")&8) out "auto-shrink"; else out "no auto shrink"


opt noerrorshere
if(empty(layout)) layout="tb"; else layout=_s.from("tb " layout)
int _1 _2 _3 style
_qmfile.SettingGetB(name layout &_1 16)
ret style
