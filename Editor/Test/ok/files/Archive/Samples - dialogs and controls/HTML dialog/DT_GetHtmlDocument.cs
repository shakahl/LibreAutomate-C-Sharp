 /
function'MSHTML.HTMLDocument hDlg ctrlid [flags] ;;flags: 1 - return 0 if not loaded

 Returns MSHTML.HTMLDocument interface pointer of a web browser control which is on a custom dialog.
 Call this function from dialog procedure.
 Returns 0 if the control currently is not displaying a html document.

 hDlg - parent dialog
 ctrlid - web browser control id


SHDocVw.WebBrowser we
we._getcontrol(id(ctrlid hDlg))
if(flags&1) if(we.Busy or we.ReadyState!=SHDocVw.READYSTATE_COMPLETE) ret
ret we.Document
