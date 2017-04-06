 /
function $urlOrFile [^waitS]

 Sends web page (URL or local file) to the default printer.

 REMARKS
 Uses a hidden web browser control. Loads web page and executes "Print" command, like in Internet Explorer.
 The command is asynchronous. This function must wait until it completes. There is no way to know when, therefore this function just wait waitS seconds. If waitS is omitted or 0, waits 1 s.
 If shows "Script Error" message box, make waitS bigger.


opt noerrorshere 1
int-- t_hax
if !t_hax
	t_hax=CreateWindowEx(0 "ActiveX" F"SHDocVw.WebBrowser" WS_POPUP 0 0 0 0 HWND_MESSAGE 0 _hinst 0)
	atend DestroyWindow t_hax

SHDocVw.WebBrowser wb._getcontrol(t_hax)
wb.Silent=1

VARIANT f=14 ;;navNoHistory|navNoReadFromCache|navNoWriteToCache. See HtmlDoc.CreateDocument.
wb.Navigate(_s.expandpath(urlOrFile) f)
opt waitmsg 1; 0; rep() if(!wb.Busy) break; else 0.01

wb.ExecWB(SHDocVw.OLECMDID_PRINT 2)

if(waitS=0) waitS=1
wait waitS
