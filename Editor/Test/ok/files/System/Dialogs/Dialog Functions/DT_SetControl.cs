 /
function# hDlg controlId $s

 Sets dialog control data like ShowDialog does.
 Returns 1. Returns 0 if the control does not exist.

 hDlg - dialog handle. If controlId 0 - control handle.
 controlId - control id, or 0 if hDlg is control handle.
 s - control data. Must have the same format as of dialog variables used with <help>ShowDialog</help> to initialize controls.

 REMARKS
 You can call this function from dialog procedure when you want to set or change control data.
 Does not change bitmaps and icons.
 Can be used in any window, not only in dialogs.

 Added in: QM 2.4.3.

 See also: <DT_SetControls> (sets multiple controls), <str.setwintext> (sets raw text).


opt noerrorshere

_s=s
ret sub_DT.SetControl(hDlg controlId _s)
