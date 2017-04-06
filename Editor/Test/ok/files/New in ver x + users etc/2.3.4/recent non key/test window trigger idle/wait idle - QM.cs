out
spe
int w

 run "winword.exe"; w=wait(0 WC "Word")
 run "excel.exe"; w=wait(0 WC "Excel")
 run "wordpad.exe"; w=wait(0 WC "WordPad")

 run "$common files$\microsoft shared\Help 9\dexplore.exe" "/helpcol ms-help://MS.W7SDK.1033 /LaunchNamedUrlTopic DefaultPage /usehelpsettings WindowsSDK.1.0" "" "" 0 win("Document Explorer" "wndclass_desked_gsk") w

 run "$program files$\Adobe\Adobe Dreamweaver CS4\Dreamweaver.exe" "" "" "" 0 win("" "_macr_dreamweaver_frame_window_") w

 run "$program files$\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.exe" "" "" "" 0 win("Microsoft Development Environment [design] - Start Page" "wndclass_desked_gsk") w
 run "$program files$\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.exe" "" "" "" 0x10000 win("Microsoft Development Environment [design] - Start Page" "wndclass_desked_gsk") w

 run "$qm$" "" "" "" 0 win("app" "" "explorer") w
 run "$system$" "" "" "" 0 win("system" "" "explorer") w
 run "$18$" "" "" "" 0 win("Network" "" "explorer") w ;;Network

 run "$program files$\Process Explorer\procexp.exe" "" "" "" 0x10000 win("Process Explorer") w

 run "$program files$\Mozilla Firefox\firefox.exe" "" "" "*" 0 "Firefox" w
 run "$program files$\Mozilla Thunderbird\thunderbird.exe" "" "" "*" 0 "Thunderbird" w

 run "$system$\cmd.exe" "" "" "%HOMEDRIVE%%HOMEPATH%" 0x10000 win("" "ConsoleWindowClass") w

 run "$program files$\Internet Explorer\IEXPLORE.EXE" "" "" "*" 0 "Internet Explorer" w
 run "$program files$\Internet Explorer\IEXPLORE.EXE" "http://www.quickmacros.com/forum" "" "*" 0 "Internet Explorer" w

 run "$local appdata$\Google\Chrome\Application\chrome.exe" "" "" "*" 0 "Chrome" w

dll "qm.exe" TestWinIdle id hwnd
TestWinIdle qmitem("window_idle") w
