 /
function hDlg flags color [color2] ;;flags: 1 horizontal gradient, 2 vertical gradient

 Sets dialog background color or gradient.

 hDlg - dialog handle.
 color - background color in format 0xBBGGRR.
 color2 - other color for gradient. Also use flag 1 or 2.

 REMARKS
 If some cases, after moving controls possible incorrect background color.
   To fix it: InvalidateRect hDlg 0 1.
   Also try to add WS_EX_TRANSPARENT style for these controls.

 Added in: QM 2.3.3.


__DIALOGCOLORS* p=sub_DT.Colors(hDlg)

p.hBrush.Delete
p.bkFlags=flags&3
if(p.bkFlags=0) p.bkFlags=3; p.hBrush=CreateSolidBrush(color)
p.color=color
p.color2=color2

if(IsWindowVisible(hDlg)) InvalidateRect hDlg 0 1
