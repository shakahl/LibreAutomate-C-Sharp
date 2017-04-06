
 max tab text length (if exceeds, cuts oldest text)
def OUTEX_MAX 4*1024*1024 ;;4 MB

 if already running, show window
if(getopt(nthreads)>1)
	act "+QM_outex"; err
	ret

#if EXE
if(!GetModuleHandle("Riched20")) LoadLibrary("Riched20")
#endif

MainWindow "QM outex" "@QM_outex" &outex_window_proc 10000 10000 600 400 WS_POPUP|WS_CAPTION|WS_SYSMENU|WS_THICKFRAME|WS_MINIMIZEBOX|WS_MAXIMIZEBOX
