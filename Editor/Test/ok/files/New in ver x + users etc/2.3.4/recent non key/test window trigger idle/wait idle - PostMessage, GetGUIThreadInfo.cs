out
spe
int w

 run "winword.exe"
w=wait(0 WC "Word")

 run "$common files$\microsoft shared\Help 9\dexplore.exe" "/helpcol ms-help://MS.W7SDK.1033 /LaunchNamedUrlTopic DefaultPage /usehelpsettings WindowsSDK.1.0" "" "" 0 win("Document Explorer" "wndclass_desked_gsk") w

 run "$program files$\Adobe\Adobe Dreamweaver CS4\Dreamweaver.exe" "" "" "" 0 win("" "_macr_dreamweaver_frame_window_") w

 PostMessage w WM_SYSCOMMAND SC_SIZE 0
act w
 PostMessage w WM_SYSKEYDOWN VK_MENU 0
 PostMessage w WM_SYSKEYUP VK_MENU 0
int tid=GetWindowThreadProcessId(w 0)
rep 10
	GUITHREADINFO g.cbSize=sizeof(g)
	if(!GetGUIThreadInfo(tid &g)) end "failed"
	outx g.flags
	
	0.05
