 /
function# [flags] ;;flags: 1 create in separate process

 Finds or creates output window.
 Returns its handle.
 To create the window, launches function outex_window. It runs in separate thread. It adds tray icon.
 When creating exe from a macro that calls outex_create_window:
    Function outex_window is not added to exe, unless you add it explicitly (#exe addfunction "outex_window").
    If the window does not exist:
        If outex_window is added to exe, launches it (creates the window in exe context).
        Else tries to find and run qm.exe, and launch outex_window in it.

 flags:
    1 - to create output window, find and run qmoutex.exe. You have to craete it from outex_window and add to a folder where it could be found by filename. If this flag is not used, launches outex_window in QM context.

 EXAMPLE
 ;show the window
 act outex_create_window



lock _outex "qm_mutex_outex"
int h=win("" "QM_outex")
if(!h)
	if(flags&1)
		run "qmoutex.exe"
	else
#if !EXE
		mac "outex_window"
#else
#opt nowarnings 1
		int iid=qmitem("outex_window")
		if(iid) mac iid
		else if(_s.searchpath("qmcl.exe")) run _s "M ''outex_window''"
#endif
	h=wait(10 WC "+QM_outex")

ret h
err+
