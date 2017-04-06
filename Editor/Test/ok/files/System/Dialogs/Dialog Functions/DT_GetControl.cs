 /
function! hDlg controlId str&s

 Gets dialog control data like ShowDialog does.
 Returns 1. Returns 0 if the control does not exist.

 hDlg - dialog handle. If controlId 0 - control handle.
 controlId - control id, or 0 if hDlg is control handle.
 s - receives control data.

 REMARKS
 You can call this function from dialog procedure when you want to get data from controls without closing the dialog.
 Gets control data in the same format as <help>ShowDialog</help> gets to populate dialog variables on OK.
 Can be used in any window, not only in dialogs.

 Added in: QM 2.4.3.

 See also: <DT_GetControls> (gets multiple controls), <str.getwintext> (gets raw text).


opt noerrorshere

ret sub_DT.GetControl(hDlg controlId s)
