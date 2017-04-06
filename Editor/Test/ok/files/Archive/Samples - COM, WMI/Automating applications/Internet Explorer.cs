 SHDocVw.InternetExplorer ie._create ;;does not work on Vista
 ie.Visible=TRUE
 act ie.HWND
SHDocVw.IWebBrowser2 ie=web("" 8) ;;works on Vista too
ie.Navigate("http://www.quickmacros.com/index.html")
5
ie.Quit
