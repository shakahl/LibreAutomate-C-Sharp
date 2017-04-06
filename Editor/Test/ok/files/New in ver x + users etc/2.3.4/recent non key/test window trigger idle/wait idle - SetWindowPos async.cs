out
spe
int w

 run "winword.exe"
 w=wait(0 WC "Word")

 run "$common files$\microsoft shared\Help 9\dexplore.exe" "/helpcol ms-help://MS.W7SDK.1033 /LaunchNamedUrlTopic DefaultPage /usehelpsettings WindowsSDK.1.0" "" "" 0 win("Document Explorer" "wndclass_desked_gsk") w

run "$program files$\Adobe\Adobe Dreamweaver CS4\Dreamweaver.exe" "" "" "" 0 win("" "_macr_dreamweaver_frame_window_") w

SetWindowPos w 0 0 0 0 0 SWP_HIDEWINDOW|SWP_ASYNCWINDOWPOS|SWP_NOSENDCHANGING|SWP_NOACTIVATE|SWP_NOSIZE|SWP_NOMOVE|SWP_NOZORDER
rep 100
	if !IsWindowVisible(w)
		 OnScreenDisplay "Loaded"
		SetWindowPos w 0 0 0 0 0 SWP_SHOWWINDOW|SWP_ASYNCWINDOWPOS|SWP_NOSENDCHANGING|SWP_NOACTIVATE|SWP_NOSIZE|SWP_NOMOVE|SWP_NOZORDER
		break
	out 1
	0.05

rep 100
	if IsWindowVisible(w)
		OnScreenDisplay "Loaded" 0 0 0 "" 0 0xff0000
		break
	out 2
	0.05
