out
spe
int w

 run "winword.exe"
 w=wait(0 WC "Word")

 run "$common files$\microsoft shared\Help 9\dexplore.exe" "/helpcol ms-help://MS.W7SDK.1033 /LaunchNamedUrlTopic DefaultPage /usehelpsettings WindowsSDK.1.0" "" "" 0 win("Document Explorer" "wndclass_desked_gsk") w

run "$program files$\Adobe\Adobe Dreamweaver CS4\Dreamweaver.exe" "" "" "" 0 win("" "_macr_dreamweaver_frame_window_") w

 run "$program files$\Adobe\Adobe Dreamweaver CS4\Dreamweaver.exe" "" "" "" 0x200 win("" "_macr_dreamweaver_frame_window_") w
 OnScreenDisplay "Loaded"
 ret

long t1 t2 td n
rep 1000
	t1=perf
	Q &q
	SendMessage w 0 0 0
	Q &qq
	outq
	td=perf-t1/10000
	out td
	if(td<1) n+1; if(n=5) OnScreenDisplay "Loaded"; break
	else n=0
	
	0.01
