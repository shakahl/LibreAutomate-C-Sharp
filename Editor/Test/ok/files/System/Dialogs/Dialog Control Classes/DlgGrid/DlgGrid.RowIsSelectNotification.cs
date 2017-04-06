function! lParam [int&row]

 Call this function when received LVN_ITEMCHANGED notification.
 If the message notifies about new item selected, returns 1, else returns 0.
 Error if the message is not LVN_ITEMCHANGED.

 lParam - lParam.
 row - receives 0-based index of the row.

 Added in: QM 2.4.2.


NMLISTVIEW* nlv=+lParam
if(nlv.hdr.code!LVN_ITEMCHANGED) goto ge

if(&row) row=nlv.iItem

ret nlv.uNewState&LVIS_SELECTED and !(nlv.uOldState&LVIS_SELECTED)

err+
	 ge
	end "call this function only on LVN_ITEMCHANGED"
