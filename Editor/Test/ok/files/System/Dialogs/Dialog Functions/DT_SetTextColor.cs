 /
function hDlg color [$controls]

 Sets text color for all or some controls in dialog.

 hDlg, controls - same as with <help>__Font.SetDialogFont</help>.
 color - text color in format 0xBBGGRR.

 REMARKS
 Can set color only of controls of these classes: Static, Edit, ListBox.
 An alternative way to set text color - in dialog procedure on WM_CTLCOLORxx messages call SetTextColor and SetBkMode. Reference - MSDN. Examples - QM forum.

 See also: <DT_SetBackgroundColor>
 Added in: QM 2.3.3.


__DIALOGCOLORS* p=sub_DT.Colors(hDlg)

ARRAY(int) a; int i j
sub_sys.Font_ParseControls(hDlg controls a)

for i 0 a.len
	for(j 0 p.a.len) if(p.a[j].x=a[i]) p.a[j].y=color; break
	if(j<p.a.len) continue
	POINT x; x.x=a[i]; x.y=color
	p.a[]=x

if(IsWindowVisible(hDlg)) InvalidateRect hDlg 0 1
