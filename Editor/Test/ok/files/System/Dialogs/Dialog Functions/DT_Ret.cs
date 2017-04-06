 /
function# hDlg retval

 Call this function to return a value from dialog procedure.

 REMARKS
 Use this function because returning a value directly does not work in most cases.
 This function calls SetWindowLong with DWL_MSGRESULT.
 Always returns 1.

 EXAMPLE
 ret DT_Ret(hDlg 10) ;;return 10


SetWindowLong hDlg DWL_MSGRESULT retval
ret 1
