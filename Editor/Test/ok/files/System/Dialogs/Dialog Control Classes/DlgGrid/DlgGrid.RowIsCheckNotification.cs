function! lParam [int&row] [int&isChecked] ;;isChecked receives: 1 checked, 0 unchecked

 Call this function when received LVN_ITEMCHANGED notification.
 If the message notifies about check state change, returns 1, else returns 0.
 Error if the message is not LVN_ITEMCHANGED.

 lParam - lParam.
 row - receives 0-based index of the row.
 isChecked - variable that receives check state.


NMLISTVIEW* nlv=+lParam
if(nlv.hdr.code!LVN_ITEMCHANGED) goto ge

if(&row) row=nlv.iItem

int os(nlv.uOldState>>12&15) ns(nlv.uNewState>>12&15)
if os or ns
	if(&isChecked) isChecked=ns=2
	ret 1

err+
	 ge
	end "call this function only on LVN_ITEMCHANGED"
