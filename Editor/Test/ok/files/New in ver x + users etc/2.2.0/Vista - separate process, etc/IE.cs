/exe 4

web "http://www.quickmacros.com" 1|8
 web "about:blank" 9

 SHDocVw.InternetExplorer ie=web("" 8)
 SHDocVw.InternetExplorer ie=web("" 8|128)
 ie.Navigate("http://www.quickmacros.com")

 web "$desktop$\Google.htm" 8
 web "$desktop$\testweb.htm" 8

 SHDocVw.InternetExplorer ie._create
 ie.Visible=TRUE
 act ie.HWND
 ie.Navigate("http://www.quickmacros.com/index.html")

 5
 ie.Quit




 BEGIN PROJECT
 main_function  IE
 exe_file  $my qm$\Macro494.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {1912DBCF-C6C5-4ABA-BA9F-D6C11459416A}
 END PROJECT
